using System.Net;
using System.Text.RegularExpressions;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Entities;
using eID.PDEAU.Service.Extensions;
using eID.PDEAU.Service.Options;
using eID.PDEAU.Service.Requests;
using eID.PDEAU.Service.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace eID.PDEAU.Service;

public class ProvidersService : BaseService, IProvidersService
{
    private const string NumberPrefix = "ПДЕАУ";

    private readonly ILogger<ProvidersService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly INotificationSender _notificationSender;
    private readonly IMpozeiCaller _mpozeiCaller;
    private readonly ITimestampService _timestampService;
    private readonly ApplicationUrls _applicationUrls;
    private readonly RedisOptions _redisOptions;
    private readonly INumberRegistrator _numberRegistrator;
    private readonly NotificationEmailsOptions _notificationEmailsOptions;

    public ProvidersService(
        ILogger<ProvidersService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IVerificationService verificationService,
        INotificationSender notificationSender,
        IMpozeiCaller mpozeiCaller,
        ITimestampService timestampService,
        INumberRegistrator numberRegistrator,
        IOptions<ApplicationUrls> applicationUrls,
        IOptions<RedisOptions> redisOptions,
        IOptions<NotificationEmailsOptions> notificationEmailsOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        _mpozeiCaller = mpozeiCaller ?? throw new ArgumentNullException(nameof(mpozeiCaller));
        _timestampService = timestampService ?? throw new ArgumentNullException(nameof(timestampService));
        _numberRegistrator = numberRegistrator ?? throw new ArgumentNullException(nameof(numberRegistrator));
        _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
        _applicationUrls.Validate();
        _redisOptions = (redisOptions ?? throw new ArgumentNullException(nameof(redisOptions))).Value;
        _notificationEmailsOptions = (notificationEmailsOptions ?? throw new ArgumentNullException(nameof(notificationEmailsOptions))).Value;
        _notificationEmailsOptions.IsValid();
    }

    public async Task<ServiceResult<bool>> ConfirmAdminPromotionAsync(ConfirmAdminPromotion message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ConfirmAdminPromotionValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<bool>(validationResult.Errors);
        }

        var administratorPromotion = await _context.ProvidersAdministratorPromotions
            .FirstOrDefaultAsync(ap => ap.Id == message.AdministratorPromotionId);

        if (administratorPromotion is null)
        {
            _logger.LogWarning("Cannot find administrator promotion with Id: {AdministratorPromotionId}.", message.AdministratorPromotionId);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"Invalid admin promotion.")
                }
            };
        }

        if (administratorPromotion.Status == AdministratorPromotionStatus.Completed)
        {
            _logger.LogWarning("Provider with Id: {ProviderId} is already completed.", administratorPromotion.ProviderId);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"This promotion is already completed.")
                }
            };
        }

        if (administratorPromotion.Status == AdministratorPromotionStatus.Denied)
        {
            _logger.LogWarning("Provider with Id: {ProviderId} is already denied.", administratorPromotion.ProviderId);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"This promotion is already denied.")
                }
            };
        }

        var oldAdmin = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => 
                u.ProviderId == administratorPromotion.ProviderId && 
                u.IsAdministrator == true && 
                u.Id == administratorPromotion.IssuerId && 
                u.IsDeleted == false);
        if (oldAdmin is null)
        {
            _logger.LogWarning("No user with Id: {UserId} found. Issuer admin is missing.", administratorPromotion.IssuerId);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"Issuer admin is missing.")
                }
            };
        }

        var newAdmin = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => 
                u.ProviderId == administratorPromotion.ProviderId && 
                u.Id == administratorPromotion.PromotedUserId && 
                u.IsDeleted == false);
        if (newAdmin is null)
        {
            _logger.LogWarning("No user with Id: {UserId} found.", administratorPromotion.PromotedUserId);
            return NotFound<bool>(nameof(administratorPromotion.PromotedUserId), administratorPromotion.PromotedUserId);
        }

        //Deny all other poending promotions for the old admin
        var oldAdminPromotions = _context.ProvidersAdministratorPromotions
            .Where(ap => ap.IssuerId == oldAdmin.Id && ap.ProviderId == administratorPromotion.ProviderId && ap.Id != administratorPromotion.Id && ap.Status == AdministratorPromotionStatus.Pending)
            .ForEachAsync(ap =>
            {
                ap.Status = AdministratorPromotionStatus.Denied;
                ap.CompletedOn = DateTime.UtcNow;
            });

        oldAdmin.IsAdministrator = false;
        newAdmin.IsAdministrator = true;
        administratorPromotion.Status = AdministratorPromotionStatus.Completed;
        administratorPromotion.CompletedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    public async Task<ServiceResult<IPaginatedData<ProviderInfoResult>>> GetAllProvidersAsync(GetAllProviders message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetAllProvidersValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<ProviderInfoResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var providers = _context.Providers
            .Where(p => p.Status == ProviderStatus.Active)
            .AsQueryable();

        // Pagination
        // Execute query
        var result = await PaginatedData<ProviderInfoResult>.CreateAsync(providers.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    private async Task<ServiceResult<IPaginatedData<T>>> GetProvidersByFilterImplAsync<T>(GetProvidersByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProvidersByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<T>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var providers = _context.Providers
            .Include(p => p.AISInformation)
            .AsQueryable();

        // This is to be used when the call is made from PDEAU Public UI
        if (!string.IsNullOrWhiteSpace(message.IssuerUid) && message.IssuerUidType != IdentifierType.NotSpecified)
        {
            providers = providers.Where(p => p.IssuerUid == message.IssuerUid && p.IssuerUidType == message.IssuerUidType);
        }

        if (message.Status.HasValue)
        {
            providers = providers.Where(u => u.Status == message.Status);
        }

        if (!string.IsNullOrWhiteSpace(message.ProviderName))
        {
            providers = providers.Where(u => u.Name.ToUpper().Contains(message.ProviderName.ToUpper()));
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                providers = message.SortBy switch
                {
                    // By default, the providers with the status "Returned for Correction" should appear at the top, while the rest should be sorted by date.
                    ProvidersSortBy.CreatedOn => providers.OrderBy(p => p.Status != ProviderStatus.ReturnedForCorrection).ThenByDescending(s => s.CreatedOn),
                    ProvidersSortBy.Name => providers.OrderByDescending(s => s.Name),
                    ProvidersSortBy.Status => providers.OrderByDescending(s => s.Status),
                    ProvidersSortBy.IssuerName => providers.OrderByDescending(s => s.IssuerName),
                    ProvidersSortBy.Number => providers.OrderByDescending(s => s.Number),
                    _ => providers.OrderBy(p => p.Status != ProviderStatus.ReturnedForCorrection).ThenByDescending(s => s.CreatedOn),
                };
            }
            else
            {
                providers = message.SortBy switch
                {
                    // By default, the providers with the status "Returned for Correction" should appear at the top, while the rest should be sorted by date.
                    ProvidersSortBy.CreatedOn => providers.OrderBy(p => p.Status != ProviderStatus.ReturnedForCorrection).ThenBy(s => s.CreatedOn),
                    ProvidersSortBy.Name => providers.OrderBy(s => s.Name),
                    ProvidersSortBy.Status => providers.OrderBy(s => s.Status),
                    ProvidersSortBy.IssuerName => providers.OrderBy(s => s.IssuerName),
                    ProvidersSortBy.Number => providers.OrderBy(s => s.Number),
                    _ => providers.OrderBy(p => p.Status != ProviderStatus.ReturnedForCorrection).ThenBy(s => s.CreatedOn),
                };
            }
        }
        else
        {
            // By default, the providers with the status "Returned for Correction" should appear at the top, while the rest should be sorted by date.
            providers = providers.OrderBy(p => p.Status != ProviderStatus.ReturnedForCorrection).ThenByDescending(s => s.CreatedOn);
        }

        if (!string.IsNullOrWhiteSpace(message.Number))
        {
            providers = providers.Where(p => p.Number.Contains(message.Number.ToUpper()));
        }

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        // Pagination
        // Execute query
        var result = await PaginatedData<T>.CreateAsync(providers.Cast<T>(), message.PageIndex, message.PageSize);
        return Ok(result);
    }

    public async Task<ServiceResult<IPaginatedData<ProviderResult>>> GetProvidersByFilterAsync(GetProvidersByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return await GetProvidersByFilterImplAsync<ProviderResult>(message);
    }

    public async Task<ServiceResult<IPaginatedData<ProviderListResult>>> GetProvidersListByFilterAsync(GetProvidersListByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return await GetProvidersByFilterImplAsync<ProviderListResult>(message);
    }

    public async Task<ServiceResult<IPaginatedData<UserResult>>> GetUsersByFilterAsync(GetUsersByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUsersByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetUsersByFilter), validationResult);
            return BadRequest<IPaginatedData<UserResult>>(validationResult.Errors);
        }

        var users = _context.ProvidersUsers
            .Where(u => u.ProviderId == message.ProviderId && u.IsDeleted == false)
            .AsQueryable();

        // Filters
        // Name
        if (!string.IsNullOrWhiteSpace(message.Name))
        {
            users = users.Where(u => u.Name.ToUpper().Contains(message.Name.ToUpper()));
        }
        // Email
        if (!string.IsNullOrWhiteSpace(message.Email))
        {
            users = users.Where(u => u.Email.ToUpper().Contains(message.Email.ToUpper()));
        }
        // IsAdministrator
        if (message.IsAdministrator.HasValue)
        {
            users = users.Where(u => u.IsAdministrator == message.IsAdministrator);
        }


        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                users = message.SortBy switch
                {
                    UsersSortBy.CreatedOn => users.OrderByDescending(s => s.CreatedOn),
                    UsersSortBy.Name => users.OrderByDescending(s => s.Name),
                    UsersSortBy.Email => users.OrderByDescending(s => s.Email),
                    UsersSortBy.Phone => users.OrderByDescending(s => s.Phone),
                    UsersSortBy.IsAdministrator => users.OrderByDescending(s => s.IsAdministrator),
                    _ => users.OrderByDescending(s => s.CreatedOn),
                };
            }
            else
            {
                users = message.SortBy switch
                {
                    UsersSortBy.CreatedOn => users.OrderBy(s => s.CreatedOn),
                    UsersSortBy.Name => users.OrderBy(s => s.Name),
                    UsersSortBy.Email => users.OrderBy(s => s.Email),
                    UsersSortBy.Phone => users.OrderBy(s => s.Phone),
                    UsersSortBy.IsAdministrator => users.OrderBy(s => s.IsAdministrator),
                    _ => users.OrderBy(s => s.CreatedOn),
                };
            }
        }
        else
        {
            users = users.OrderByDescending(s => s.CreatedOn);
        }

        // Execute query
        var result = await PaginatedData<UserResult>.CreateAsync(users, message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<UserResult>> GetUserDetailsAsync(GetUserDetails message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserDetailsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetUserDetailsAsync), validationResult);
            return BadRequest<UserResult>(validationResult.Errors);
        }

        var user = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId && u.Id == message.UserId && u.IsDeleted == false);

        if (user is null)
        {
            _logger.LogWarning("No user with Id: {UserId} found.", message.UserId);
            return NotFound<UserResult>(nameof(message.UserId), message.UserId);
        }

        _logger.LogInformation("User with Id: {UserId} found.", message.UserId);
        return Ok<UserResult>(user);
    }

    public async Task<ServiceResult<Guid>> InitiateAdminPromotionAsync(InitiateAdminPromotion message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new InitiateAdminPromotionValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(InitiateAdminPromotion), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var provider = await _context.Providers
            .Include(p => p.Users.Where(u => u.IsDeleted == false))
            .FirstOrDefaultAsync(x => x.Id == message.ProviderId);

        //Check if Provider exists
        if (provider == null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound<Guid>(nameof(message.ProviderId), message.ProviderId);
        }

        //Check if Requester is the Provider's admin
        var issuer = provider.Users
            .FirstOrDefault(u => u.IsAdministrator && u.Uid == message.IssuerUid && u.UidType == message.IssuerUidType && u.Name == message.IssuerName);

        if (issuer is null)
        {
            var maskedUid = Regex.Replace(message.IssuerUid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            var maskedNames = message.IssuerName.Split(" ").Select(name => name[0] + new string('*', name.Length - 1));
            var maskedName = string.Join(" ", maskedNames);
            _logger.LogWarning("Issuer: {IssuerName} with Uid {MaskedUid} {UidType} not found for provider {ProviderId}", maskedName, maskedUid, message.IssuerUidType, provider.Id);

            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"You can't promote users.")
                }
            };
        }

        //Check if Requester is the target for promotion
        if (issuer.Id == message.UserId)
        {
            _logger.LogWarning("User with Id: {UserId} tried to promote himself to Administrator.", message.UserId);

            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"You can't promote yourself to administrator.")
                }
            };
        }

        //Check if the promoting User is part of the registered Users in Provider 
        var targetUser = provider.Users.FirstOrDefault(u => u.Id == message.UserId);
        if (targetUser is null)
        {
            _logger.LogWarning("No user with Id: {UserId} found. Cannot be promoted to Administrator.", message.UserId);
            return NotFound<Guid>(nameof(message.UserId), message.UserId);
        }

        //Check if the promoting User has email
        if (string.IsNullOrWhiteSpace(targetUser.Email))
        {
            _logger.LogWarning("User: {UserId} does not have email. Cannot be promoted to Administrator.", message.UserId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"You can't promote administrator that have no registered email.")
                }
            };
        }

        var administratorPromotion = new AdministratorPromotion
        {
            Id = Guid.NewGuid(),
            ProviderId = message.ProviderId,
            IssuerId = issuer.Id,
            PromotedUserId = targetUser.Id,
            CreatedOn = DateTime.UtcNow,
            Status = AdministratorPromotionStatus.Pending
        };

        await _context.ProvidersAdministratorPromotions.AddAsync(administratorPromotion);
        await _context.SaveChangesAsync();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // When there's an issue with email sending, waiting could timeout the http request.
        Task.Run(async () =>
        {
            var confirmationEmailSend = await _notificationSender.SendConfirmationEmailAsync(new SendConfirmationEmailRequest
            {
                CorrelationId = message.CorrelationId,
                Uid = targetUser.Uid,
                UidType = targetUser.UidType,
                AdministratorPromotionId = administratorPromotion.Id
            });

            if (!confirmationEmailSend.Result)
            {
                _logger.LogWarning("Failed to send Confirmation email to {UserId}", message.UserId);
            }
        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        return Ok(administratorPromotion.Id);
    }

    public async Task<ServiceResult<Guid>> RegisterProviderAsync(RegisterProvider message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterProviderValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(RegisterProvider), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var mpozeiResult = await _mpozeiCaller.FetchUserProfileAsync(message.CorrelationId, message.Administrator.Uid, message.Administrator.UidType);
        if (!mpozeiResult.ok)
        {
            var maskedUid = Regex.Replace(message.Administrator.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} does not own Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("Eid", $"User with Uid: {maskedUid} does not own Electronic identity Id");
        }

        if (!mpozeiResult.userProfile.Active)
        {
            var maskedUid = Regex.Replace(message.Administrator.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} does not own Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("EidInactive", $"User with Uid: {maskedUid} has inactive Electronic identity Id");
        }

        var eid = mpozeiResult.userProfile.EidentityId;
        var createdOn = DateTime.UtcNow;
        var number = await BuildNumberAsync(createdOn);
        try
        {
            var provider = new Provider
            {
                Id = message.ProviderId,
                Number = number,
                ExternalNumber = message.ExternalNumber,
                IssuerUid = message.IssuerUid,
                IssuerUidType = message.IssuerUidType,
                IssuerName = message.IssuerName,
                Name = message.Name,
                Bulstat = message.Bulstat,
                Headquarters = message.Headquarters,
                Address = message.Address,
                Email = message.Email,
                Phone = message.Phone,
                CreatedOn = createdOn,
                Status = ProviderStatus.InReview,
                Type = message.Type,
                GeneralInformation = string.Empty // It will be filled later
            };

            if (!string.IsNullOrWhiteSpace(message.CreatedByAdministratorId))
            {
                provider.CreatedByAdministratorId = message.CreatedByAdministratorId;
            }

            if (!string.IsNullOrWhiteSpace(message.CreatedByAdministratorName))
            {
                provider.CreatedByAdministratorName = message.CreatedByAdministratorName;
            }

            provider.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Uid = message.Administrator.Uid,
                UidType = message.Administrator.UidType,
                EID = eid,
                Name = message.Administrator.Name,
                Email = message.Administrator.Email,
                Phone = message.Administrator.Phone,
                IsAdministrator = true,
                CreatedOn = provider.CreatedOn,
                IsDeleted = false
            });

            provider.StatusHistory.Add(new ProviderStatusHistory
            {
                Id = Guid.NewGuid(),
                DateTime = provider.CreatedOn,
                ModifierUid = provider.IssuerUid,
                ModifierUidType = provider.IssuerUidType,
                ModifierFullName = provider.IssuerName,
                Status = provider.Status
            });

            // Timestamp signing
            var providerTimestampData = ProviderTimestampData.Create(provider);
            var xmlRepresentation = providerTimestampData.Serialize();
            var signResult = await _timestampService.SignDataAsync(xmlRepresentation);
            if (signResult.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning("Sign provider data is not successful. Error: {Error}", signResult.Error);

                return new ServiceResult<Guid> { Error = signResult.Error, StatusCode = signResult.StatusCode, Errors = signResult.Errors };
            }

            provider.XMLRepresentation = xmlRepresentation;

            provider.Timestamp = new ProviderTimestamp
            {
                Signature = signResult.Result,
                DateTime = DateTime.UtcNow,
            };

            if (message.Type == ProviderType.Administration)
            {
                var providerDetails = await _context.ProvidersDetails.FirstOrDefaultAsync(pd => pd.Name.ToUpper() == provider.Name.ToUpper());
                if (providerDetails is not null)
                {
                    provider.Details = providerDetails;
                    provider.DetailsId = providerDetails.Id;
                    provider.IdentificationNumber = providerDetails.IdentificationNumber;
                }
                else
                {
                    _logger.LogWarning("During registration provider details {Provider} not found.", provider.Name);
                }
            }
            else
            {
                var providerDetailsEntity = new ProviderDetails
                {
                    UIC = provider.Bulstat,
                    Name = provider.Name,
                    Status = ProviderDetailsStatus.Deactivated,
                    SyncedFromOnlineRegistry = false,
                    Address = provider.Address,
                    Headquarters = provider.Headquarters
                };

                provider.Details = providerDetailsEntity;
            }

            // Register files
            var fileInformation = message.FilesInformation;
            foreach (var fileData in fileInformation.Files)
            {
                provider.Files.Add(new ProviderFile
                {
                    ProviderId = message.ProviderId,
                    FileName = fileData.FileName,
                    FilePath = fileData.FullFilePath,
                    UploaderUid = fileInformation.UploaderUid,
                    UploaderUidType = fileInformation.UploaderUidType,
                    UploaderName = fileInformation.UploaderName,
                    UploadedOn = DateTime.UtcNow
                });
            }

            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully pass all checks and successfully registered new Provider: {ProviderId}", provider.Id);

            SendEmail(message.CorrelationId, _notificationEmailsOptions.DEAUActions,
                "Заявление за регистрация на ДЕАУ", "bg", $"Подадено е заявление за регистрация на ДЕАУ от {message.Name}.",
                "Failed to send MoI registered provider email. ProviderId {ProviderId} MoI email {MoIEmail}",
                    provider.Id, _notificationEmailsOptions.DEAUActions.MaskEmail());

            return Created(provider.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during provider registration.");
            return UnhandledException<Guid>();
        }
    }

    public async Task<ServiceResult<Guid>> RegisterUserAsync(RegisterUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(RegisterUser), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogInformation("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound<Guid>(nameof(message.ProviderId), message.ProviderId);
        }

        var existingUser = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId &&
                                      u.Uid == message.Uid &&
                                      u.UidType == message.UidType && 
                                      u.IsDeleted == false);

        if (existingUser is not null)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} already exists. Provider: {ProviderId}", maskedUid, message.ProviderId);
            return Conflict<Guid>(nameof(message.Uid), message.Uid);
        }

        var mpozeiResult = await _mpozeiCaller.FetchUserProfileAsync(message.CorrelationId, message.Uid, message.UidType);
        if (!mpozeiResult.ok)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} does not own Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("Eid", $"User with Uid: {maskedUid} does not own Electronic identity Id");
        }

        if (!mpozeiResult.userProfile.Active)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} has inactive Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("EidInactive", $"User with Uid: {maskedUid} has inactive Electronic identity Id");
        }

        var eid = mpozeiResult.userProfile.EidentityId;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Uid = message.Uid,
            UidType = message.UidType,
            EID = eid,
            Name = message.Name,
            Email = message.Email,
            Phone = message.Phone,
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false,
            ProviderId = provider.Id
        };

        await _context.ProvidersUsers.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New user with Id: {UserId} has been registered", user.Id);
        return Created(user.Id);
    }

    public async Task<ServiceResult<Guid>> AdministratorRegisterUserAsync(AdministratorRegisterUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new AdministratorRegisterUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(AdministratorRegisterUser), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound<Guid>(nameof(message.ProviderId), message.ProviderId);
        }

        var existingUser = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId &&
                                      u.Uid == message.Uid &&
                                      u.UidType == message.UidType && 
                                      u.IsDeleted == false);

        if (existingUser is not null)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid}, Name: {Name} already exists. Provider: {ProviderId}", maskedUid, message.Name, message.ProviderId);
            return Conflict<Guid>(nameof(message.Uid), message.Uid);
        }

        var mpozeiResult = await _mpozeiCaller.FetchUserProfileAsync(message.CorrelationId, message.Uid, message.UidType);
        if (!mpozeiResult.ok)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} does not own Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("Eid", $"User with Uid: {maskedUid} does not own Electronic identity Id");
        }

        if (!mpozeiResult.userProfile.Active)
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("User with Uid: {Uid} has inactive Electronic identity Id", maskedUid);
            return ConflictMessage<Guid>("EidInactive", $"User with Uid: {maskedUid} has inactive Electronic identity Id");
        }

        var eid = mpozeiResult.userProfile.EidentityId;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Uid = message.Uid,
            UidType = message.UidType,
            EID = eid,
            Name = message.Name,
            Email = message.Email,
            Phone = message.Phone,
            CreatedOn = DateTime.UtcNow,
            ProviderId = provider.Id,
            IsAdministrator = message.IsAdministrator,
            IsDeleted = false
        };

        user.AdministratorActions.Add(new AdministratorAction
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            AdministratorUid = message.AdministratorUid,
            AdministratorUidType = message.AdministratorUidType,
            AdministratorFullName = message.AdministratorFullName,
            Comment = message.Comment,
            Action = AdministratorActionType.Register
        });

        await _context.ProvidersUsers.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Administrator has registered new user with Id: {UserId}", user.Id);
        return Created(user.Id);
    }

    public async Task<ServiceResult<ProviderFileResult>> GetProviderFileAsync(GetProviderFile message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderFileValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetProviderFileAsync), validationResult);
            return BadRequest<ProviderFileResult>(validationResult.Errors);
        }

        var providers = _context.Providers.AsQueryable();

        if (message.IsPublic && message.ProviderId == Guid.Empty)
        {
            // This is has not been confirmed provider yet without ProviderId
            providers = providers.Where(p => p.Files.Any(f => f.Id == message.FileId));
        }
        else
        {
            providers = providers.Where(p => p.Id == message.ProviderId);
        }

        // This is to be used when the call is made from PDEAU Public UI
        if (!string.IsNullOrWhiteSpace(message.IssuerUid) && message.IssuerUidType != IdentifierType.NotSpecified)
        {
            providers = providers.Where(p => p.IssuerUid == message.IssuerUid && p.IssuerUidType == message.IssuerUidType);
        }

        var provider = await providers.FirstOrDefaultAsync();
        if (provider is null)
        {
            _logger.LogInformation("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound<ProviderFileResult>(nameof(message.ProviderId), message.ProviderId);
        }

        var file = await _context.ProvidersFiles.FirstOrDefaultAsync(file => file.Id == message.FileId && file.ProviderId == provider.Id);
        if (file is null)
        {
            _logger.LogInformation("No file with Id: {FileId} found.", message.FileId);
            return NotFound<ProviderFileResult>(nameof(message.FileId), message.FileId);
        }

        return Ok<ProviderFileResult>(file);
    }

    public async Task<ServiceResult<AdministratorRegisteredProviderResult>> GetProviderByIdAsync(GetProviderById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetProviderByIdAsync), validationResult);
            return BadRequest<AdministratorRegisteredProviderResult>(validationResult.Errors);
        }

        var providers = _context.Providers
                        .Include(pr => pr.Users.Where(u => u.IsDeleted == false))
                        .Include(pr => pr.Files)
                        .Include(pr => pr.AISInformation)
                        .Where(pr => pr.Id == message.Id);

        // This is to be used when the call is made from PDEAU Public UI
        if (!string.IsNullOrWhiteSpace(message.IssuerUid) && message.IssuerUidType != IdentifierType.NotSpecified)
        {
            providers = providers.Where(p => p.IssuerUid == message.IssuerUid && p.IssuerUidType == message.IssuerUidType);
        }

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers.FirstOrDefaultAsync();
        if (provider is null)
        {
            _logger.LogInformation("No provider with Id: {ProviderId} found.", message.Id);
            return NotFound<AdministratorRegisteredProviderResult>(nameof(message.Id), message.Id);
        }

        return Ok<AdministratorRegisteredProviderResult>(provider);
    }

    private async Task DenyProviderByIdAsync(Guid providerId)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId);
        if (provider is null)
        {
            _logger.LogInformation("Cannot find provider with Id: {ProviderId}", providerId);
            return;
        }

        provider.Status = ProviderStatus.Denied;

        _context.ProvidersStatusHistory.Add(new ProviderStatusHistory
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            ModifierUid = provider.IssuerUid,
            ModifierUidType = provider.IssuerUidType,
            ModifierFullName = provider.IssuerName,
            Status = provider.Status,
            ProviderId = provider.Id
        });

        await _context.SaveChangesAsync();
    }

    public async Task<ServiceResult> UpdateUserAsync(UpdateUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(UpdateUser), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogInformation("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        var existingUser = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId &&
                                      u.Id == message.Id && 
                                      u.IsDeleted == false);
        if (existingUser is null)
        {
            _logger.LogInformation("User Id: {UserId} does not exists. Provider: {ProviderId}", message.Id, message.ProviderId);
            return NotFound(nameof(message.Id), message.Id);
        }

        var remainingAdministrators = await _context.ProvidersUsers
            .CountAsync(pu => pu.ProviderId == message.ProviderId &&
                              pu.Id != message.Id &&
                              pu.IsAdministrator == true && 
                              pu.IsDeleted == false);

        if (existingUser.IsAdministrator && !message.IsAdministrator && remainingAdministrators < 1)
        {
            _logger.LogWarning("Cannot change the role of the only administrator. UserId: {UserId} for ProviderId: {ProviderId}", message.Id, message.ProviderId);
            return Conflict(nameof(message.Id), message.Id, "Cannot change the role of the only administrator.");
        }

        existingUser.Name = message.Name;
        existingUser.Uid = message.Uid;
        existingUser.UidType = message.UidType;
        existingUser.Email = message.Email;
        existingUser.Phone = message.Phone;
        existingUser.IsAdministrator = message.IsAdministrator;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User with Id: {UserId} has been updated", existingUser.Id);
        return Ok();
    }

    public async Task<ServiceResult> DeleteUserAsync(DeleteUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DeleteUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(DeleteUser), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var user = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId &&
                                      u.Id == message.Id && 
                                      u.IsDeleted == false);
        if (user is null)
        {
            _logger.LogWarning("User Id: {UserId} for Provider: {ProviderId} does not exists.", message.Id, message.ProviderId);
            return NotFound(nameof(message.Id), message.Id);
        }

        var remainingAdministrators = await _context.ProvidersUsers
            .CountAsync(pu => pu.ProviderId == message.ProviderId &&
                              pu.Id != message.Id &&
                              pu.IsAdministrator == true && 
                              pu.IsDeleted == false);
        if (remainingAdministrators < 1)
        {
            _logger.LogWarning("The only administrator cannot be deleted. UserId: {UserId} for ProviderId: {ProviderId}", message.Id, message.ProviderId);
            return Conflict(nameof(message.Id), message.Id, "The only administrator cannot be deleted.");
        }

        user.IsDeleted = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("UserId: {UserId} for ProviderId: {ProviderId} has been soft deleted", message.Id, message.ProviderId);

        return NoContent();
    }

    public async Task<ServiceResult> AdministratorUpdateUserAsync(AdministratorUpdateUser message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new AdministratorUpdateUserValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(AdministratorUpdateUser), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var existingUser = await _context.ProvidersUsers
            .FirstOrDefaultAsync(u => u.ProviderId == message.ProviderId &&
                                      u.Id == message.Id && 
                                      u.IsDeleted == false);

        if (existingUser is null)
        {
            _logger.LogWarning("User Id: {UserId} does not exists. Provider: {ProviderId}", message.Id, message.ProviderId);
            return NotFound(nameof(message.Id), message.Id);
        }

        var remainingAdministrators = await _context.ProvidersUsers
            .CountAsync(pu => pu.ProviderId == message.ProviderId &&
                              pu.Id != message.Id &&
                              pu.IsAdministrator == true && 
                              pu.IsDeleted == false);

        if (existingUser.IsAdministrator && !message.IsAdministrator && remainingAdministrators < 1)
        {
            _logger.LogWarning("Cannot change the role of the only administrator. UserId: {UserId} for ProviderId: {ProviderId}", message.Id, message.ProviderId);
            return Conflict(nameof(message.Id), message.Id, "Cannot change the role of the only administrator.");
        }

        // Action
        existingUser.IsAdministrator = message.IsAdministrator;

        await _context.AdministratorActions.AddAsync(new AdministratorAction
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            AdministratorUid = message.AdministratorUid,
            AdministratorUidType = message.AdministratorUidType,
            AdministratorFullName = message.AdministratorFullName,
            Comment = message.Comment,
            Action = AdministratorActionType.Update,
            UserId = existingUser.Id
        });

        await _context.SaveChangesAsync();

        // Result
        _logger.LogInformation("Administrator has updated User with Id: {UserId}", existingUser.Id);

        return Ok();
    }

    public async Task<ServiceResult> ApproveProviderAsync(ApproveProvider message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ApproveProviderValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ApproveProvider), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var providers = _context.Providers
            .Include(d => d.Users.Where(u => u.IsDeleted == false && u.IsAdministrator).OrderBy(o => o.CreatedOn))
            .Include(p => p.Details)
            .AsQueryable();

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers.FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        if (provider.Status == ProviderStatus.Active)
        {
            _logger.LogWarning("Provider with Id: {ProviderId} already in status {Status}.", provider.Id, ProviderStatus.Active);
            return Conflict(nameof(provider.Id), provider.Id, $"already in status {ProviderStatus.Active}");
        }

        if (provider.Status != ProviderStatus.InReview)
        {
            _logger.LogWarning("Provider with Id {ProviderId} and status {CurrentStatus} must be in {Status} to be Approved.", provider.Id, provider.Status, ProviderStatus.InReview);
            return Conflict(nameof(provider.Id), provider.Id, $"and status {provider.Status} must be in {ProviderStatus.InReview} to be Approved");
        }

        // Check if there is another active Provider with the same provider detail
        var providerWithTheSameService = await _context.Providers.FirstOrDefaultAsync(p =>
            p.Status == ProviderStatus.Active &&
            p.DetailsId == provider.DetailsId &&
            p.Id != provider.Id);

        if (providerWithTheSameService is not null)
        {
            _logger.LogWarning("Provider with Id {ProviderId} is tried to activate the same service as {ProviderWithTheSameServiceId}.", provider.Id, providerWithTheSameService.Id);
            return Conflict(nameof(provider.Id), provider.Id, $"is tried to activate the same service as {providerWithTheSameService.Id}");
        }

        // Action
        provider.Status = ProviderStatus.Active;
        _context.ProvidersStatusHistory.Add(new ProviderStatusHistory
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            ModifierUid = message.IssuerUid,
            ModifierUidType = message.IssuerUidType,
            ModifierFullName = message.IssuerName,
            Status = provider.Status,
            Comment = message.Comment,
            ProviderId = provider.Id
        });

        if (provider.Details is not null)
        {
            provider.Details.Status = ProviderDetailsStatus.Active;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Provider with Id: {ProviderId} has been approved", provider.Id);

        await DeleteCacheKeysAsync(CacheKeyHelper.ProvidersInfoBase);

        // Send emails
        // Issuer
        SendEmail(message.CorrelationId, provider.Email, "ДЕАУ е одобрен.", "bg",
            "Вашата регистрация като доставчик на административни електронни услуги в централната система за електронна идентичност е одобрена.",
            "Failed to send provider issuer approved email Uid: {Uid} UidType: {UidType}",
                provider.IssuerUid.Mask(), provider.IssuerUidType);

        // Admin
        if (!provider.Users.Any())
        {
            _logger.LogWarning("Administrator for ProviderId {ProviderId} is not found", provider.Id);
        }
        else
        {
            var adminEmail = provider.Users.First().Email;
            SendEmail(message.CorrelationId, adminEmail, "ДЕАУ е одобрен.", "bg",
                "Регистриран сте като администратор на доставчик на административни електронни услуги в централната система за електронна идентичност." +
                    $" Моля, използвайте вашата идентичност за достъп до <a href=\"{_applicationUrls.PdeauUiUrl}\">портала за доставчици на административни електронни услуги</a>.",
                "Failed to send provider admin approved email for ProviderId {ProviderId} admin email {AdminEmail}",
                    provider.Id, adminEmail.MaskEmail());
        }

        // Result
        return Ok();
    }

    /// <summary>
    /// Support return provider corrections.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<ServiceResult> ReturnProviderForCorrectionAsync(ReturnProviderForCorrection message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ReturnProviderForCorrectionValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ReturnProviderForCorrection), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var providers = _context.Providers
            .Include(d => d.Users.Where(u => u.IsDeleted == false && u.IsAdministrator).OrderBy(o => o.CreatedOn))
            .AsQueryable();

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers.FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        if (provider.Status == ProviderStatus.ReturnedForCorrection)
        {
            _logger.LogWarning("Provider with Id: {ProviderId} already in status {Status}", provider.Id, ProviderStatus.ReturnedForCorrection);
            return Conflict(nameof(provider.Id), provider.Id, $"already in status {ProviderStatus.ReturnedForCorrection}");
        }

        if (provider.Status != ProviderStatus.InReview)
        {
            _logger.LogWarning("Provider with Id {ProviderId} and status {CurrentStatus} must be in {Status} to be Returned for correction.", provider.Id, provider.Status, ProviderStatus.InReview);
            return Conflict(nameof(provider.Id), provider.Id, $"is in status {provider.Status}. Only {ProviderStatus.InReview} can be Returned for correction");
        }

        // Action
        provider.Status = ProviderStatus.ReturnedForCorrection;
        _context.ProvidersStatusHistory.Add(new ProviderStatusHistory
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            ModifierUid = message.IssuerUid,
            ModifierUidType = message.IssuerUidType,
            ModifierFullName = message.IssuerName,
            Status = provider.Status,
            Comment = message.Comment,
            ProviderId = provider.Id
        });

        await _context.SaveChangesAsync();

        _logger.LogInformation("Provider with Id: {ProviderId} has been Returned for correction", provider.Id);

        // Send emails
        // Issuer
        SendEmail(message.CorrelationId, provider.Email, "ДЕАУ е върната за корекция.", "bg",
            "Вашата регистрация като доставчик на административни електронни услуги в централната система за електронна идентичност е върната за корекция.",
            "Failed to send provider issuer returned for correction email Uid: {Uid} UidType: {UidType}",
                provider.IssuerUid.Mask(), provider.IssuerUidType);

        // Admin
        if (!provider.Users.Any())
        {
            _logger.LogWarning("Administrator for ProviderId {ProviderId} is not found", provider.Id);
        }
        else
        {
            var adminEmail = provider.Users.First().Email;

            SendEmail(message.CorrelationId, adminEmail, "ДЕАУ е върната за корекция.", "bg",
                "Регистрацията за доставчик на административни електронни услуги, към която сте добавен като администратор е върната за корекция.",
                "Failed to send provider admin returned for correction email for ProviderId {ProviderId} admin email {AdminEmail}",
                    provider.Id, adminEmail.MaskEmail());
        }

        // Result
        return Ok();
    }

    public async Task<ServiceResult> UpdateProviderAsync(UpdateProvider message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateProviderValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(UpdateProvider), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        if (provider.Status != ProviderStatus.ReturnedForCorrection)
        {
            _logger.LogWarning("Provider with Id {ProviderId} and status {CurrentStatus} must be in {Status} to be Updated.", provider.Id, provider.Status, ProviderStatus.ReturnedForCorrection);
            return Conflict(nameof(provider.Id), provider.Id, $"and status {provider.Status} must be in {ProviderStatus.ReturnedForCorrection} to be updated");
        }

        // Action
        provider.IssuerUid = message.IssuerUid;
        provider.IssuerUidType = message.IssuerUidType;
        provider.IssuerName = message.IssuerName;
        provider.Status = ProviderStatus.InReview;

        _context.ProvidersStatusHistory.Add(new ProviderStatusHistory
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            ModifierUid = provider.IssuerUid,
            ModifierUidType = provider.IssuerUidType,
            ModifierFullName = provider.IssuerName,
            Status = provider.Status,
            Comment = message.Comment,
            ProviderId = provider.Id
        });


        // Register files
        var fileInformation = message.FilesInformation;
        foreach (var fileData in fileInformation.Files)
        {
            provider.Files.Add(new ProviderFile
            {
                ProviderId = message.ProviderId,
                FileName = fileData.FileName,
                FilePath = fileData.FullFilePath,
                UploaderUid = fileInformation.UploaderUid,
                UploaderUidType = fileInformation.UploaderUidType,
                UploaderName = fileInformation.UploaderName,
                UploadedOn = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        SendEmail(message.CorrelationId, _notificationEmailsOptions.DEAUActions,
            "Заявление за регистрация на ДЕАУ", "bg", $"Заявление за регистрация на ДЕАУ от {provider.Name} е върнато за разглеждане.",
            "Failed to send MoI returned for review email for ProviderId {ProviderId} MoI email {MoIEmail}",
            provider.Id, _notificationEmailsOptions.DEAUActions.MaskEmail());

        _logger.LogInformation("Successfully pass all checks and successfully updated Provider: {ProviderId}", provider.Id);
        return Ok();
    }

    public async Task<ServiceResult<IEnumerable<ProviderStatusHistoryResult>>> GetProviderStatusHistoryAsync(GetProviderStatusHistory message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderStatusHistoryValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetProviderStatusHistoryAsync), validationResult);
            return BadRequest<IEnumerable<ProviderStatusHistoryResult>>(validationResult.Errors);
        }
        var providersStatusHistory = _context.ProvidersStatusHistory
          .Where(d => d.ProviderId == message.ProviderId)
          .AsQueryable();

        if (message.IsPLSRole)
        {
            providersStatusHistory = providersStatusHistory.Where(psh => psh.Provider.Type == ProviderType.PrivateLawSubject);
        }

        var result = await providersStatusHistory
          .OrderByDescending(d => d.DateTime)
          .Cast<ProviderStatusHistoryResult>()
          .ToArrayAsync();

        return Ok(result.AsEnumerable());
    }

    public async Task<ServiceResult> DenyProviderAsync(DenyProvider message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DenyProviderValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(DenyProvider), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var providers = _context.Providers
            .Include(d => d.Users.Where(u => u.IsDeleted == false && u.IsAdministrator).OrderBy(o => o.CreatedOn))
            .Include(p => p.Details)
            .AsQueryable();

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers.FirstOrDefaultAsync(p => p.Id == message.ProviderId);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        if (provider.Status == ProviderStatus.Denied)
        {
            _logger.LogWarning("Provider with Id: {ProviderId} already in status {Status}.", provider.Id, ProviderStatus.Denied);
            return Conflict(nameof(provider.Id), provider.Id, $"already in status {ProviderStatus.Denied}");
        }

        // Action
        provider.Status = ProviderStatus.Denied;
        _context.ProvidersStatusHistory.Add(new ProviderStatusHistory
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            ModifierUid = message.IssuerUid,
            ModifierUidType = message.IssuerUidType,
            ModifierFullName = message.IssuerName,
            Status = provider.Status,
            Comment = message.Comment,
            ProviderId = provider.Id
        });

        if (provider.Details is not null
            && provider.Details.Status == ProviderDetailsStatus.Active)
        {
            provider.Details.Status = ProviderDetailsStatus.Deactivated;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Provider with Id: {ProviderId} has been denied", provider.Id);

        await DeleteCacheKeysAsync(CacheKeyHelper.ProvidersInfoBase);

        // Send emails
        // Issuer

        SendEmail(message.CorrelationId, provider.Email, "ДЕАУ е отказан.", "bg",
            "Вашата регистрация като доставчик на административни електронни услуги в централната система за електронна идентичност е отказана.",
            "Failed to send provider issuer denied email Uid: {Uid} UidType: {UidType}",
                provider.IssuerUid.Mask(), provider.IssuerUidType);

        // Admin
        if (!provider.Users.Any())
        {
            _logger.LogWarning("Administrator for ProviderId {ProviderId} is not found", provider.Id);
        }
        else
        {
            var adminEmail = provider.Users.First().Email;
            SendEmail(message.CorrelationId, adminEmail, "ДЕАУ е отказан.", "bg",
                "Регистрацията за доставчик на административни електронни услуги, към която сте добавен като администратор е отказана.",
                "Failed to send provider admin denied email for ProviderId {ProviderId} admin email {AdminEmail}",
                    provider.Id, adminEmail.MaskEmail());
        }

        // Result
        return Ok();
    }

    public async Task<ServiceResult<ProviderGeneralInformationAndOfficesResult>> GetProviderGeneralInformationAndOfficesByIdAsync(GetProviderGeneralInformationAndOfficesById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderGeneralInformationAndOfficesByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetProviderGeneralInformationAndOfficesByIdAsync), validationResult);
            return BadRequest<ProviderGeneralInformationAndOfficesResult>(validationResult.Errors);
        }

        var provider = await _context.Providers
                        .Include(pr => pr.Offices.OrderBy(pr => pr.Name))
                        .Where(pr => pr.Id == message.Id)
                        .FirstOrDefaultAsync();

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.Id);
            return NotFound<ProviderGeneralInformationAndOfficesResult>(nameof(message.Id), message.Id);
        }

        return Ok<ProviderGeneralInformationAndOfficesResult>(provider);
    }

    public async Task<ServiceResult> UpdateProviderGeneralInformationAndOfficesAsync(UpdateProviderGeneralInformationAndOffices message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateProviderGeneralInformationAndOfficesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(UpdateProviderGeneralInformationAndOffices), validationResult);
            return BadRequest(validationResult.Errors);
        }

        // Action
        var provider = await _context.Providers
            .Include(pr => pr.Offices)
            .FirstOrDefaultAsync(p => p.Id == message.Id);

        if (provider is null)
        {
            _logger.LogWarning("No provider with Id: {ProviderId} found.", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        provider.GeneralInformation = message.GeneralInformation;

        // Update offices
        var deletedOffices = provider.Offices.Where(d => !message.Offices.Any(mo => ProvidersEqual(mo, d))).ToArray();
        foreach (var item in deletedOffices)
        {
            provider.Offices.Remove(item);
        }

        var newOffices = message.Offices.Where(d => !provider.Offices.Any(po => ProvidersEqual(po, d)));
        _context.ProvidersOffices.AddRange(newOffices.Select(d => new ProviderOffice
        {
            Id = Guid.NewGuid(),
            Name = d.Name,
            Address = d.Address,
            Lat = d.Lat,
            Lon = d.Lon,
            ProviderId = provider.Id,
            Provider = provider
        }));

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKeyHelper.GetProviderOffices(message.Id));
        await DeleteCacheKeysAsync(CacheKeyHelper.ProvidersInfoBase);

        _logger.LogInformation("For provider with Id: {ProviderId} has been updated General information and Offices", provider.Id);

        return NoContent();
    }

    private static bool ProvidersEqual(IProviderOffice left, IProviderOffice right)
    {
        return left.Name == right.Name &&
               left.Address == right.Address &&
               left.Lat == right.Lat &&
               left.Lon == right.Lon;
    }

    public async Task<ServiceResult<IPaginatedData<ProviderInfoResult>>> GetProvidersInfoAsync(GetProvidersInfo message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProvidersInfoValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<ProviderInfoResult>>(validationResult.Errors);
        }

        // Action
        var key = CacheKeyHelper.GetProvidersInfo(message.PageIndex, message.PageSize, message.Name,
            message.IdentificationNumber, message.Bulstat, message.SortBy, message.SortDirection);
        var providers = await _cache.GetOrAddDefAsync(key, async () =>
            {
                var query = _context.Providers
                    .Where(p => p.Status == ProviderStatus.Active)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(message.Name))
                {
                    query = query.Where(p => p.Name.ToUpper().Contains(message.Name.ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(message.IdentificationNumber))
                {
                    query = query.Where(p => p.IdentificationNumber.Contains(message.IdentificationNumber));
                }

                if (!string.IsNullOrWhiteSpace(message.Bulstat))
                {
                    query = query.Where(p => p.Bulstat == message.Bulstat);
                }

                query = message.SortBy switch
                {
                    GetProvidersInfoSortBy.Name => message.SortDirection == SortDirection.Asc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                    GetProvidersInfoSortBy.IdentificationNumber => message.SortDirection == SortDirection.Asc ? query.OrderBy(s => s.IdentificationNumber) : query.OrderByDescending(s => s.IdentificationNumber),
                    GetProvidersInfoSortBy.Bulstat => message.SortDirection == SortDirection.Asc ? query.OrderBy(s => s.Bulstat) : query.OrderByDescending(s => s.Bulstat),
                    _ => message.SortDirection == SortDirection.Asc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                };

                return await PaginatedData<Provider>.CreatePaginatedDataAsync(query, message.PageIndex, message.PageSize);
            },
            PaginatedData<Provider>.EmptyPaginatedData(message.PageIndex),
            slidingExpiration: TimeSpan.FromMinutes(10)
        );

        var result = PaginatedData<ProviderInfoResult>.Create(providers);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IEnumerable<IProviderOffice>>> GetProviderOfficesAsync(GetProviderOffices message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderOfficesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<IProviderOffice>>(validationResult.Errors);
        }

        // Action
        var key = CacheKeyHelper.GetProviderOffices(message.Id);
        var providerOffices = await _cache.GetOrAddDefAsync(key, async () =>
            {
                return await _context.Providers
                    .Include(db => db.Offices)
                    .Where(p => p.Id == message.Id && p.Status == ProviderStatus.Active)
                    .SelectMany(p => p.Offices.Select(po => po))
                    .OrderBy(po => po.Name)
                    .ToListAsync();
            },
            Enumerable.Empty<ProviderOffice>().ToList(),
            slidingExpiration: TimeSpan.FromMinutes(10)
        );

        // Result
        return Ok(providerOffices.Cast<IProviderOffice>());
    }

    public async Task<ServiceResult<IEnumerable<ProviderServiceInfoResult>>> GetProviderServicesAsync(GetProviderServices message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderServicesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<ProviderServiceInfoResult>>(validationResult.Errors);
        }

        // Action
        var key = CacheKeyHelper.GetProviderServices(message.Id);
        var providerServices = await _cache.GetOrAddDefAsync(key, async () =>
            {
                var providerDetailsId = await _context.Providers
                    .Where(pr => pr.Id == message.Id && pr.Status == ProviderStatus.Active)
                    .Select(p => p.DetailsId)
                    .FirstOrDefaultAsync();

                if (providerDetailsId is null)
                {
                    return Enumerable.Empty<ProviderService>().ToList();
                }

                return await _context.ProvidersDetailsServices
                    .Where(ps => ps.ProviderDetailsId == providerDetailsId
                        && ps.ServiceScopes.Any()
                        && ps.IsEmpowerment == true
                        && ps.MinimumLevelOfAssurance != LevelOfAssurance.None
                        && ps.RequiredPersonalInformation != null
                        && ps.IsDeleted == false)
                    .OrderBy(ps => ps.Name)
                    .ToListAsync();
            },
            Enumerable.Empty<ProviderService>().ToList(),
            slidingExpiration: TimeSpan.FromMinutes(10)
        );

        // Result
        return Ok(providerServices.Cast<ProviderServiceInfoResult>());
    }

    public async Task<ServiceResult<UserByUidResult>> GetUserByUidAsync(GetUserByUid message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserByUidValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetUserByUid), validationResult);
            return BadRequest<UserByUidResult>(validationResult.Errors);
        }

        var users = _context.ProvidersUsers
                .Where(u =>
                    u.Uid == message.Uid &&
                    u.UidType == message.UidType &&
                    u.ProviderId == message.ProviderId &&
                    u.Provider.Status == ProviderStatus.Active && 
                    u.IsDeleted == false)
            .Include(u => u.Provider)
            .AsQueryable();

        // Execute query
        UserByUidResult? result = await users
            .Select(u =>
                new UserByUidDTO
                {
                    Uid = u.Uid,
                    UidType = u.UidType,
                    ProviderId = u.ProviderId,
                    ProviderName = u.Provider.Name,
                    IsAdministrator = u.IsAdministrator,
                })
            .FirstOrDefaultAsync();

        // Result
        if (result == null)
        {
            return NotFound<UserByUidResult>(nameof(message.Uid), message.Uid);
        }

        return Ok(result);
    }

    public async Task<ServiceResult<IEnumerable<AdministratorActionResult>>> GetUserAdministratorActionsAsync(GetUserAdministratorActions message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetUserAdministratorActionsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(AdministratorActionResult), validationResult);
            return BadRequest<IEnumerable<AdministratorActionResult>>(validationResult.Errors);
        }

        IEnumerable<AdministratorActionResult> data = await _context.AdministratorActions
                .Where(u =>
                    u.User.ProviderId == message.ProviderId &&
                    u.UserId == message.UserId &&
                    u.User.Provider.Status == ProviderStatus.Active)
                .ToListAsync();

        return Ok(data);
    }

    public async Task<ServiceResult> RegisterDoneServiceAsync(RegisterDoneService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterDoneServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(RegisterDoneService), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var providerDetails = await _context.ProvidersDetails
            .Include(pd => pd.ProviderServices.Where(ps => ps.Name.ToLower() == message.ServiceName.ToLower()))
            .FirstOrDefaultAsync(pd => pd.Providers.Any(p => p.Id == message.ProviderId));

        if (providerDetails is null)
        {
            _logger.LogInformation("No provider with Id: {ProviderId} found.", message.ProviderId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        var providerService = providerDetails.ProviderServices.FirstOrDefault();
        if (providerService is null)
        {
            _logger.LogInformation("No service with name: {ServiceName} found.", message.ServiceName);
            return NotFound(nameof(message.ServiceName), message.ServiceName);
        }

        var doneService = new ProviderDoneService
        {
            Id = Guid.NewGuid(),
            ProviderId = message.ProviderId,
            ServiceId = providerService.Id,
            Count = message.Count,
            CreatedOn = DateTime.UtcNow
        };

        await _context.ProviderDoneServices.AddAsync(doneService);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New done service with Id: {Id} has been registered", doneService.Id);
        return Created();
    }

    private async Task DeleteCacheKeysAsync(string keyPreffix)
    {
        var options = ConfigurationOptions.Parse(_redisOptions.ConnectionString);

        using (var redis = ConnectionMultiplexer.Connect(options))
        {
            var db = redis.GetDatabase();
            var server = redis.GetServer(options.EndPoints.First());
            var keys = server.Keys(database: db.Database, pattern: $"{keyPreffix}*").ToArray();

            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }
    private async Task<string> BuildNumberAsync(DateTime dateTime)
    {
        var currentNumber = await _numberRegistrator.GetProviderRegistrationNextNumberAsync(dateTime);

        return $"{NumberPrefix}{currentNumber}/{dateTime:dd.MM.yyyy}";
    }

    private void SendEmail(Guid correlationId, string email, string subject, string language, string body, string? message, params object?[] args)
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // When there's an issue with email sending, waiting could timeout the http request.
        Task.Run(async () =>
        {
            var confirmationEmailSend = await _notificationSender.SendEmailAsync(new SendEmailRequest
            {
                CorrelationId = correlationId,
                Email = email,
                Subject = subject,
                Language = language,
                Body = body
            });

            if (!confirmationEmailSend)
            {
                _logger.LogWarning(message, args);
            }
        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
}

internal class UserByUidDTO : UserByUidResult
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public bool IsAdministrator { get; set; }
}
