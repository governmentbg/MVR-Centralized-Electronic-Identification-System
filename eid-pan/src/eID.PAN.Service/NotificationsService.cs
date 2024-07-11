using System.Net;
using System.Text.RegularExpressions;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Enums;
using eID.PAN.Contracts.Results;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Requests;
using eID.PAN.Service.Validators;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eID.PAN.Service;

public class NotificationsService : BaseService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConfiguration _configuration;
    private readonly IMpozeiCaller _mpozeiCaller;
    private readonly HttpClient _httpClient;

    public NotificationsService(
        ILogger<NotificationsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMpozeiCaller mpozeiCaller)
    {

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _mpozeiCaller = mpozeiCaller ?? throw new ArgumentNullException(nameof(mpozeiCaller));
        _httpClient = httpClientFactory.CreateClient(ApplicationPolicyRegistry.HttpClientWithRetryPolicy);
    }

    public async Task<ServiceResult<IPaginatedData<RegisteredSystemResult>>> GetByFilterAsync(GetNotificationsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetNotificationsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<RegisteredSystemResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var registeredSystems = _context.RegisteredSystems
            .Include(rs => rs.Events)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(message.SystemName))
        {
            registeredSystems = registeredSystems
                .Where(s => s.Name.Contains(message.SystemName));
        }

        // if message.IncludeDeleted returns all
        if (!message.IncludeDeleted)
        {
            registeredSystems = registeredSystems
                .Where(s => s.IsDeleted == false);
        }

        // Pagination
        // Execute query
        var result = await PaginatedData<RegisteredSystemResult>.CreateAsync(registeredSystems.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<RegisteredSystemResult>> GetSystemByIdAsync(GetSystemById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSystemByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<RegisteredSystemResult>(validationResult.Errors);
        }

        // Action
        var result = await _context.RegisteredSystems
            .Include(rs => rs.Events)
            .FirstOrDefaultAsync(rs => rs.Id == message.Id);

        if (result is null)
        {
            return NotFound<RegisteredSystemResult>(nameof(message.Id), message.Id);
        }

        // Result
        return Ok((RegisteredSystemResult)result);
    }

    public async Task<ServiceResult<Guid>> RegisterSystemAsync(RegisterSystem message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterSystemValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Action
        // Change data char cases
        message.SystemName = message.SystemName.ToUpperInvariant();
        message.Events.ToList().ForEach(ev =>
        {
            ev.Translations.ToList()
                .ForEach(tr => tr.Language = tr.Language.ToLowerInvariant());
        });

        var registeredSystem = await _context.RegisteredSystems
            .Include(rs => rs.Events)
            .FirstOrDefaultAsync(rs => rs.Name == message.SystemName);

        var translations = message.Translations.Select(t =>
            new Entities.RegisteredSystemTranslation
            {
                Name = t.Name,
                Language = t.Language
            });

        // Is the system is new
        if (registeredSystem == null)
        {
            registeredSystem = new RegisteredSystem
            {
                Id = Guid.NewGuid(),
                Name = message.SystemName,
                ModifiedBy = message.ModifiedBy,
                ModifiedOn = DateTime.UtcNow,
                IsApproved = false,
                IsDeleted = false,
            };

            translations.ToList()
                .ForEach(t => registeredSystem.Translations.Add(t));

            message.Events.ToList()
                .ForEach(e => registeredSystem.Events.Add(CreateFromMessageEvent(e)));

            _context.RegisteredSystems.Add(registeredSystem);
        }
        else
        {
            if (!Enumerable.SequenceEqual(registeredSystem.Translations, translations))
            {
                registeredSystem.Translations.Clear();
                translations.ToList()
                    .ForEach(t => registeredSystem.Translations.Add(t));
                // Npgsql bug. EF don't track "jsonb" properties
                _context.Entry(registeredSystem).Property(b => b.Translations).IsModified = true;

                registeredSystem.ModifiedBy = message.ModifiedBy;
                registeredSystem.ModifiedOn = DateTime.UtcNow;
            }

            // Process update events
            registeredSystem.Events.ToList()
                .ForEach(rse =>
                {
                    var me = message.Events.FirstOrDefault(e => CompareCode(e.Code, rse.Code));
                    if (me != null)
                    {
                        var newMessageEvent = CreateFromMessageEvent(me);
                        if (newMessageEvent != rse)
                        {
                            rse.Code = newMessageEvent.Code;
                            rse.IsMandatory = newMessageEvent.IsMandatory;
                            rse.IsDeleted = newMessageEvent.IsDeleted;
                            rse.Translations = newMessageEvent.Translations;
                        }
                    }
                });

            // Process deleted events
            var deletedEvents = registeredSystem.Events
                .Where(re => !message.Events.Any(e => CompareCode(e.Code, re.Code)));
            if (deletedEvents.Any())
            {
                _context.RemoveRange(deletedEvents);
            }

            // Process new events
            var newEvents = message.Events
                .Where(re => !registeredSystem.Events.Any(e => CompareCode(e.Code, re.Code)));
            if (newEvents.Any())
            {
                _context.SystemEvents.AddRange(newEvents.Select(ne =>
                {
                    var e = CreateFromMessageEvent(ne);
                    e.RegisteredSystemId = registeredSystem.Id;
                    return e;
                }));
            }
        }

        // Execute query
        await _context.SaveChangesAsync();

        // Result
        return Ok(registeredSystem.Id);
    }

    public async Task<ServiceResult> ModifyEventAsync(ModifyEvent message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ModifyEventValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Process
        var systemEvent = await _context.SystemEvents.FirstOrDefaultAsync(se => se.Id == message.Id);
        if (systemEvent is null)
        {
            return NotFound(nameof(message.Id), message.Id);
        }

        if (systemEvent.IsDeleted == message.IsDeleted)
        {
            return NotModified();
        }

        systemEvent.IsDeleted = message.IsDeleted;
        systemEvent.ModifiedBy = message.ModifiedBy;
        systemEvent.ModifiedOn = DateTime.UtcNow;

        // Execute
        await _context.SaveChangesAsync();

        // Result
        return NoContent();
    }

    public async Task<ServiceResult<Guid>> RejectSystemAsync(RejectSystem message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RejectSystemValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Action
        var registeredSystem = await _context.RegisteredSystems.SingleOrDefaultAsync(s => s.Id == message.SystemId);
        if (registeredSystem is null)
        {
            return NotFound<Guid>(nameof(message.SystemId), message.SystemId);
        }
        if (registeredSystem.IsApproved)
        {
            return new ServiceResult<Guid>
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("System", "Already approved")
                }
            };
        }
        if (registeredSystem.IsDeleted)
        {
            return new ServiceResult<Guid>
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("System", "Archived")
                }
            };
        }

        var registeredSystemRejected = new RegisteredSystemRejected
        {
            Id = Guid.NewGuid(),
            Name = registeredSystem.Name,
            RejectedBy = message.UserId,
            RejectedOn = DateTime.UtcNow,
            Translations = registeredSystem.Translations,
        };

        // Execute
        _context.RegisteredSystems.Remove(registeredSystem);
        await _context.RegisteredSystemsRejected.AddAsync(registeredSystemRejected);
        await _context.SaveChangesAsync();

        // Result
        return Ok(registeredSystemRejected.Id);
    }

    public async Task<ServiceResult<Guid>> ApproveSystemAsync(ApproveSystem message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ApproveSystemValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Action
        var registeredSystem = await _context.RegisteredSystems.SingleOrDefaultAsync(s => s.Id == message.SystemId);
        if (registeredSystem is null)
        {
            return NotFound<Guid>(nameof(message.SystemId), message.SystemId);
        }
        if (registeredSystem.IsDeleted)
        {
            return new ServiceResult<Guid>
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("System", "Archived")
                }
            };
        }
        if (registeredSystem.IsApproved)
        {
            return NotModified(message.SystemId);
        }

        registeredSystem.IsApproved = true;
        registeredSystem.ModifiedBy = message.UserId;
        registeredSystem.ModifiedOn = DateTime.UtcNow;

        // Execute
        _context.RegisteredSystems.Update(registeredSystem);
        await _context.SaveChangesAsync();

        // Result
        return Ok(registeredSystem.Id);
    }

    public async Task<ServiceResult<Guid>> ArchiveSystemAsync(ArchiveSystem message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ArchiveSystemValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Action
        var registeredSystem = await _context.RegisteredSystems.SingleOrDefaultAsync(s => s.Id == message.SystemId);
        if (registeredSystem is null)
        {
            return NotFound<Guid>(nameof(message.SystemId), message.SystemId);
        }
        if (!registeredSystem.IsApproved)
        {
            return new ServiceResult<Guid>
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("System", "Not approved")
                }
            };
        }
        if (registeredSystem.IsDeleted)
        {
            return NotModified(message.SystemId);
        }

        registeredSystem.IsDeleted = true;
        registeredSystem.ModifiedBy = message.UserId;
        registeredSystem.ModifiedOn = DateTime.UtcNow;

        // Execute
        _context.RegisteredSystems.Update(registeredSystem);
        await _context.SaveChangesAsync();

        // Result
        return Ok(registeredSystem.Id);
    }

    public async Task<ServiceResult<Guid>> RestoreSystemAsync(RestoreSystem message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RestoreSystemValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        // Action
        var registeredSystem = await _context.RegisteredSystems.SingleOrDefaultAsync(s => s.Id == message.SystemId);
        if (registeredSystem is null)
        {
            return NotFound<Guid>(nameof(message.SystemId), message.SystemId);
        }
        if (!registeredSystem.IsApproved)
        {
            return new ServiceResult<Guid>
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("System", "Not approved")
                }
            };
        }
        if (!registeredSystem.IsDeleted)
        {
            return NotModified(message.SystemId);
        }

        registeredSystem.IsDeleted = false;
        registeredSystem.ModifiedBy = message.UserId;
        registeredSystem.ModifiedOn = DateTime.UtcNow;

        // Execute
        _context.RegisteredSystems.Update(registeredSystem);
        await _context.SaveChangesAsync();

        // Result
        return Ok(registeredSystem.Id);
    }

    public async Task<ServiceResult<bool>> SendAsync(SendNotification message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendNotificationValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SendNotification), validationResult);
            return BadRequest<bool>(validationResult.Errors);
        }

        var registeredSystem = await _context.RegisteredSystems.FirstOrDefaultAsync(x => x.Name == message.SystemName);
        if (registeredSystem is null)
        {
            _logger.LogWarning("Notification will not be sent. {SystemName} does not exist in the system", message.SystemName);
            return NotFound<bool>($"{nameof(message.SystemName)}", $"{message.SystemName}");
        }

        if (!registeredSystem.IsApproved)
        {
            _logger.LogWarning("Notification will not be sent. {SystemProperty}:{SystemName} is not approved",
                nameof(message.SystemName), message.SystemName);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Conflict", $"'{message.SystemName}' is not approved")
                }
            };
        }

        if (registeredSystem.IsDeleted)
        {
            _logger.LogWarning("Notification will not be sent. {SystemName} is archived", message.SystemName);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Conflict", $"'{message.SystemName}' is archived")
                }
            };
        }

        //TODO: Refactor this when the user profile is available
        Guid userId = Guid.Empty;
        if (message.UserId.HasValue)
        {
            userId = message.UserId.Value;
        }
        else
        {
            MpozeiUserProfile userProfile;
            if (!string.IsNullOrWhiteSpace(message.Uid))
            {
                //User profile in Mpozei by EGN or LNCh
                userProfile = await _mpozeiCaller.FetchUserProfileAsync(message.Uid, message.UidType);
                if (userProfile is null)
                {
                    var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
                    _logger.LogWarning("There was a problem while trying to obtain User profile in Mpozei by UId: {Uid}", maskedUid);
                    return NotFound<bool>($"Uid", $"{message.Uid}");
                }

                userId = Guid.Parse(userProfile.EidentityId);
            }
            else if (message.EId.HasValue)
            {
                //User profile in Mpozei by EId
                userProfile = await _mpozeiCaller.FetchUserProfileAsync(message.EId.Value);
                if (userProfile is null)
                {
                    _logger.LogWarning("There was a problem while trying to obtain User profile in Mpozei by EId: {EId}", message.EId);
                    return NotFound<bool>($"EId", $"{message.EId}");
                }

                userId = Guid.Parse(userProfile.EidentityId);
            }
        }

        var notificationEvent = await _context.SystemEvents
                .Include(t => t.RegisteredSystem)
                .Include(t => t.DeactivatedUserEvent.Where(due => due.UserId == userId))
                .Where(ev => ev.Code == message.EventCode)
                .Where(ev => ev.RegisteredSystem.Name == message.SystemName)
                .FirstOrDefaultAsync();

        if (notificationEvent is null)
        {
            _logger.LogWarning("Notification will not be sent. {SystemName}:{EventCode} does not exist in the system",
                message.SystemName, message.EventCode);
            return NotFound<bool>($"{nameof(message.SystemName)}:{nameof(message.EventCode)}", $"{message.SystemName}:{message.EventCode}");
        }

        if (notificationEvent.IsDeleted)
        {
            _logger.LogInformation("Notification will not be sent. {SystemName}:{EventCode} is deleted",
                message.SystemName, message.EventCode);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Conflict", $"'{message.SystemName}:{message.EventCode}' is archived")
                }
            };
        }

        if (!notificationEvent.IsMandatory && notificationEvent.DeactivatedUserEvent.Any())
        {
            _logger.LogInformation("Notification will not be sent. {SystemName}:{EventCode} is deactivated from the user {UserId}",
                message.SystemName, message.EventCode, userId);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Deactivated", $"'{message.SystemName}:{message.EventCode}' is deactivated by the user")
                }
            };
        }

        // Get notification channels and user preferences
        var notificationChannels = await _context.NotificationChannels
            .Include(nc => nc.UserNotificationChannels
                .Where(unc => unc.UserId == userId))
            .ToListAsync();
        if (notificationChannels is null || !notificationChannels.Any())
        {
            _logger.LogWarning("{NotificationChannels} are empty", nameof(_context.NotificationChannels));
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Empty", "There are no channels to send the notification.")
                }
            };
        }

        // Prepare user notification channels
        var userNotificationChannels = notificationChannels.Where(nc => nc.UserNotificationChannels.Any()).ToList();
        if (!userNotificationChannels.Any())
        {
            // Use default channel
            var defaultNotificationChannel = notificationChannels.FirstOrDefault(nc => nc.IsBuiltIn && nc.Name == ConfigurationsConstants.SMTP);

            if (defaultNotificationChannel is null)
            {
                _logger.LogError("Default notification channel ({DefaultChannelName}) is missing", ConfigurationsConstants.SMTP);

                return new ServiceResult<bool>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Errors = new List<KeyValuePair<string, string>>
                    {
                        new ("Default channel", $"Unable to get default notification channel.")
                    }
                };
            }

            _logger.LogInformation("User {UserId} did not select any {UserNotificationChannels}. It has been fell back to {DefaultNotificationChannel}",
                userId, nameof(_context.UserNotificationChannels), ConfigurationsConstants.SMTP);

            userNotificationChannels.Add(defaultNotificationChannel);
        }

        // Prepare notification content
        var request = new SendNotificationRequest { UserId = userId };
        // Priority is get the content from the message
        if (message.Translations is not null && message.Translations.Any())
        {
            request.Translations = message.Translations.Select(c => new SendNotificationContentTranslation
            {
                Language = c.Language,
                Message = c.Message
            });
        }
        else
        {
            request.Translations = notificationEvent.Translations.Select(c => new SendNotificationContentTranslation
            {
                Language = c.Language,
                Message = c.Description
            });
        }
        _ = int.TryParse(_configuration["NotificationContentMaxLength"], out var configNotificationContentMaxLength);
        // Trim translations
        var notificationContentMaxLength = Math.Max(2, configNotificationContentMaxLength);

        if (request.Translations.Any(t => t.Message.Length > notificationContentMaxLength))
        {
            _logger.LogInformation("Notification will be trimmed to {NotificationContentMaxLength} chars", notificationContentMaxLength);
            request.Translations.ToList().ForEach(t =>
            {
                // If the string is shorter we get ArgumentOutOfRangeException
                if (t.Message.Length > notificationContentMaxLength)
                {
                    t.Message = t.Message[..notificationContentMaxLength];
                }
            });
        }

        var sendNotificationTasks = new List<Task>();

        foreach (var channel in userNotificationChannels)
        {
            Task sendTask;
            if (!channel.IsBuiltIn)
            {
                sendTask = _publishEndpoint.Publish<SendHttpCallbackAsync>(new
                {
                    message.CorrelationId,
                    request.UserId,
                    channel.CallbackUrl,
                    Body = request,
                    request.Translations
                });
            }
            else
            {
                sendTask = channel.Name switch
                {
                    ConfigurationsConstants.SMTP => _publishEndpoint.Publish<SendEmail>(new
                    {
                        message.CorrelationId,
                        request.UserId,
                        request.Translations
                    }),
                    ConfigurationsConstants.PUSH => _publishEndpoint.Publish<SendPushNotification>(new
                    {
                        message.CorrelationId,
                        request.UserId,
                        request.Translations
                    }),
                    ConfigurationsConstants.SMS => _publishEndpoint.Publish<SendSms>(new
                    {
                        message.CorrelationId,
                        request.UserId,
                        request.Translations
                    }),
                    _ => _publishEndpoint.Publish<SendHttpCallbackAsync>(new
                    {
                        message.CorrelationId,
                        request.UserId,
                        channel.CallbackUrl,
                        Body = request,
                        request.Translations
                    })
                };
            }
            sendNotificationTasks.Add(sendTask);
        }

        await Task.WhenAll(sendNotificationTasks);
        return Accepted(true);
    }

    public async Task<ServiceResult<IPaginatedData<RegisteredSystemRejectedResult>>> GetRegisteredSystemsRejectedAsync(GetRegisteredSystemsRejected message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetRegisteredSystemsRejectedValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<RegisteredSystemRejectedResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var registeredSystemsRejected = _context.RegisteredSystemsRejected.AsQueryable();

        // Pagination
        // Execute query
        var result = await PaginatedData<RegisteredSystemRejectedResult>.CreateAsync(registeredSystemsRejected.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }
    public async Task<ServiceResult<IPaginatedData<RegisteredSystemResult>>> GetSystemsByFilterAsync(GetSystemsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSystemsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<RegisteredSystemResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var registeredSystems = _context.RegisteredSystems
            .Include(rs => rs.Events)
            .AsQueryable();

        switch (message.RegisteredSystemState)
        {
            case RegisteredSystemState.Pending:
                registeredSystems = registeredSystems.Where(rs => !rs.IsDeleted && !rs.IsApproved);
                break;
            case RegisteredSystemState.Approved:
                registeredSystems = registeredSystems.Where(rs => !rs.IsDeleted && rs.IsApproved);
                break;
            case RegisteredSystemState.Archived:
                registeredSystems = registeredSystems.Where(rs => rs.IsDeleted);
                break;
        }

        // Pagination
        // Execute query
        var result = await PaginatedData<RegisteredSystemResult>.CreateAsync(registeredSystems.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    private static bool CompareCode(string left, string right)
        => left.ToLowerInvariant() == right.ToLowerInvariant();

    private static Entities.SystemEvent CreateFromMessageEvent(Contracts.Commands.SystemEvent se)
    {
        var result = new Entities.SystemEvent
        {
            Code = se.Code,
            IsDeleted = false,
            IsMandatory = se.IsMandatory,
        };

        se.Translations.ToList().ForEach(t =>
            result.Translations.Add(
                new Entities.Translation
                {
                    Language = t.Language,
                    ShortDescription = t.ShortDescription,
                    Description = t.Description
                })
            );

        return result;
    }







}
