using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using eID.PJS.AuditLogging;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Entities;
using eID.RO.Service.EventsRegistration;
using eID.RO.Service.Extensions;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Requests;
using eID.RO.Service.Responses;
using eID.RO.Service.Validators;
using FluentValidation;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Npgsql;

namespace eID.RO.Service;

public class EmpowermentsService : BaseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmpowermentsService> _logger;
    private readonly IAuditLogger _auditLogger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRequestClient<CheckUidsRestrictions> _checkUidsRestrictionsClient;
    private readonly IVerificationService _verificationService;
    private readonly INumberRegistrator _numberRegistrator;
    private readonly INotificationSender _notificationSender;
    protected HttpClient _httpClient;

    private const string NumberPrefix = "РО";
    private readonly IRequestClient<CheckForEmpowermentsVerification> _checkEmpowermentVerificationSagasClient;

    public EmpowermentsService(
        IConfiguration configuration,
        ILogger<EmpowermentsService> logger,
        IAuditLogger auditLogger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IPublishEndpoint publishEndpoint,
        IRequestClient<CheckUidsRestrictions> checkUidsRestrictionsClient,
        IVerificationService verificationService,
        IHttpClientFactory httpClientFactory,
        INumberRegistrator numberRegistrator,
        INotificationSender notificationSender,
        IRequestClient<CheckForEmpowermentsVerification> checkEmpowermentVerificationSagasClient
        )
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _checkUidsRestrictionsClient = checkUidsRestrictionsClient ?? throw new ArgumentNullException(nameof(checkUidsRestrictionsClient));
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        _httpClient = httpClientFactory.CreateClient("Signing");
        _numberRegistrator = numberRegistrator ?? throw new ArgumentNullException(nameof(numberRegistrator));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        _checkEmpowermentVerificationSagasClient = checkEmpowermentVerificationSagasClient ?? throw new ArgumentNullException(nameof(checkEmpowermentVerificationSagasClient));
    }

    public async Task<ServiceResult<IEnumerable<Guid>>> CreateStatementAsync(AddEmpowermentStatement message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new AddEmpowermentStatementValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(AddEmpowermentStatement), validationResult);
            return BadRequest<IEnumerable<Guid>>(validationResult.Errors);
        }

        // Action
        DateTime startDate = message.StartDate.ToUniversalTime();
        DateTime? expiryDate = null;
        if (message.ExpiryDate.HasValue)
        {
            expiryDate = message.ExpiryDate.Value.ToUniversalTime();
        }
        var authorizedUids = message.AuthorizerUids.Select(uid => new AuthorizerUid { Id = Guid.NewGuid(), Uid = uid.Uid, UidType = uid.UidType, Name = uid.Name });
        string? issuerPosition = null;
        if (message.OnBehalfOf == OnBehalfOf.LegalEntity)
        {
            issuerPosition = message.IssuerPosition;
        }
        var newRecordsData = new List<InitiateEmpowermentActivationProcess>();
        if (message.TypeOfEmpowerment == TypeOfEmpowerment.Together)
        {
            var empowermentId = Guid.NewGuid();
            var createdOn = DateTime.UtcNow;
            var number = await BuildNumberAsync(createdOn);
            var empowermentStatement = new EmpowermentStatement
            {
                Id = empowermentId,
                Number = number,
                IssuerPosition = issuerPosition,
                CreatedBy = message.CreatedBy,
                CreatedOn = createdOn,
                Uid = message.Uid,
                UidType = message.UidType,
                Name = message.Name,
                OnBehalfOf = message.OnBehalfOf,
                AuthorizerUids = authorizedUids.ToList(),
                EmpoweredUids = message.EmpoweredUids.Select(uid => new EmpoweredUid { Id = Guid.NewGuid(), Uid = uid.Uid, UidType = uid.UidType, Name = uid.Name }).ToList(),
                ProviderId = message.ProviderId,
                ProviderName = message.ProviderName,
                ServiceId = message.ServiceId,
                ServiceName = message.ServiceName,
                VolumeOfRepresentation = message.VolumeOfRepresentation.Select(vor => new VolumeOfRepresentation
                {
                    Name = vor.Name
                }).ToList(),
                StartDate = startDate,
                ExpiryDate = expiryDate,
                XMLRepresentation = XMLSerializationHelper.SerializeEmpowermentStatementItem(
                    new EmpowermentStatementItem
                    {
                        Id = empowermentId.ToString(),
                        Number = number,
                        CreatedOn = createdOn,
                        OnBehalfOf = message.OnBehalfOf.ToString(),
                        Uid = message.Uid,
                        UidType = message.UidType,
                        Name = message.Name,
                        AuthorizerUids = message.AuthorizerUids.Select(x => new AuthorizerIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }).ToArray(),
                        EmpoweredUids = message.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }).ToArray(),
                        ProviderId = message.ProviderId,
                        ProviderName = message.ProviderName,
                        ServiceId = message.ServiceId,
                        ServiceName = message.ServiceName,
                        TypeOfEmpowerment = message.TypeOfEmpowerment.ToString(),
                        VolumeOfRepresentation = message.VolumeOfRepresentation.Select(v => new VolumeOfRepresentationItem { Name = v.Name }).ToArray(),
                        StartDate = startDate,
                        ExpiryDate = expiryDate,
                    }),
                Status = EmpowermentStatementStatus.Created
            };

            empowermentStatement.StatusHistory.Add(new StatusHistoryRecord
            {
                Id = Guid.NewGuid(),
                DateTime = createdOn,
                Status = EmpowermentStatementStatus.Created
            });

            _context.EmpowermentStatements.Add(empowermentStatement);
            newRecordsData.Add(new EmpowermentActivationProcessData
            {
                CorrelationId = message.CorrelationId,
                EmpowermentId = empowermentStatement.Id,
                OnBehalfOf = empowermentStatement.OnBehalfOf,
                IssuerPosition = message.IssuerPosition,
                IssuerName = message.AuthorizerUids.Single(a => a.IsIssuer).Name,
                Uid = empowermentStatement.Uid,
                UidType = empowermentStatement.UidType,
                Name = empowermentStatement.Name,
                AuthorizerUids = message.AuthorizerUids.Select(x => new AuthorizerIdentifierData
                {
                    Uid = x.Uid,
                    UidType = x.UidType,
                    Name = x.Name,
                    IsIssuer = x.IsIssuer
                }),
                EmpoweredUids = empowermentStatement.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }),
                ExpiryDate = empowermentStatement.ExpiryDate,
            });
        }
        else
        {
            foreach (var empoweredUid in message.EmpoweredUids)
            {
                var empowermentId = Guid.NewGuid();
                var createdOn = DateTime.UtcNow;
                var number = await BuildNumberAsync(createdOn);
                var empowermentStatement = new EmpowermentStatement
                {
                    Id = empowermentId,
                    Number = number,
                    IssuerPosition = issuerPosition,
                    CreatedBy = message.CreatedBy,
                    CreatedOn = createdOn,
                    Uid = message.Uid,
                    UidType = message.UidType,
                    Name = message.Name,
                    OnBehalfOf = message.OnBehalfOf,
                    AuthorizerUids = authorizedUids.ToList(),
                    EmpoweredUids = new List<EmpoweredUid> { new() { Id = Guid.NewGuid(), Uid = empoweredUid.Uid, UidType = empoweredUid.UidType, Name = empoweredUid.Name } },
                    ProviderId = message.ProviderId,
                    ProviderName = message.ProviderName,
                    ServiceId = message.ServiceId,
                    ServiceName = message.ServiceName,
                    VolumeOfRepresentation = message.VolumeOfRepresentation.Select(vor => new VolumeOfRepresentation
                    {
                        Name = vor.Name
                    }).ToList(),
                    StartDate = startDate,
                    ExpiryDate = expiryDate,
                    XMLRepresentation = XMLSerializationHelper.SerializeEmpowermentStatementItem(
                    new EmpowermentStatementItem
                    {
                        Id = empowermentId.ToString(),
                        Number = number,
                        CreatedOn = createdOn,
                        OnBehalfOf = message.OnBehalfOf.ToString(),
                        Uid = message.Uid,
                        UidType = message.UidType,
                        Name = message.Name,
                        AuthorizerUids = message.AuthorizerUids.Select(x => new AuthorizerIdentifierData
                        {
                            Uid = x.Uid,
                            UidType = x.UidType,
                            Name = x.Name,
                            IsIssuer = x.IsIssuer
                        }).ToArray(),
                        EmpoweredUids = new[] { new UserIdentifierData { Uid = empoweredUid.Uid, UidType = empoweredUid.UidType, Name = empoweredUid.Name } },
                        ProviderId = message.ProviderId,
                        ProviderName = message.ProviderName,
                        ServiceId = message.ServiceId,
                        ServiceName = message.ServiceName,
                        TypeOfEmpowerment = message.TypeOfEmpowerment.ToString(),
                        VolumeOfRepresentation = message.VolumeOfRepresentation.Select(v => new VolumeOfRepresentationItem { Name = v.Name }).ToArray(),
                        StartDate = startDate,
                        ExpiryDate = expiryDate
                    }),
                    Status = EmpowermentStatementStatus.Created
                };

                empowermentStatement.StatusHistory.Add(new StatusHistoryRecord
                {
                    Id = Guid.NewGuid(),
                    DateTime = createdOn,
                    Status = EmpowermentStatementStatus.Created,
                });

                _context.EmpowermentStatements.Add(empowermentStatement);
                newRecordsData.Add(new EmpowermentActivationProcessData
                {
                    CorrelationId = message.CorrelationId,
                    EmpowermentId = empowermentStatement.Id,
                    OnBehalfOf = empowermentStatement.OnBehalfOf,
                    IssuerPosition = message.IssuerPosition,
                    IssuerName = message.AuthorizerUids.Single(a => a.IsIssuer).Name,
                    Uid = empowermentStatement.Uid,
                    UidType = empowermentStatement.UidType,
                    Name = empowermentStatement.Name,
                    AuthorizerUids = message.AuthorizerUids.Select(x => new AuthorizerIdentifierData
                    {
                        Uid = x.Uid,
                        UidType = x.UidType,
                        Name = x.Name,
                        IsIssuer = x.IsIssuer
                    }),
                    EmpoweredUids = empowermentStatement.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }),
                    ExpiryDate = empowermentStatement.ExpiryDate,
                });
            }
        }
        // Execute query
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {EmpowermentsCount} {OnBehalfOf} empowerments.", newRecordsData.Count, message.OnBehalfOf);

        var publishTasks = new List<Task>();
        foreach (var newRecord in newRecordsData)
        {
            var task = _publishEndpoint.Publish(newRecord);
            publishTasks.Add(task);
        }
        await Task.WhenAll(publishTasks);

        var messageTasks = new List<Task>();
        foreach (var newRecord in newRecordsData)
        {
            // Notify Authorizers
            var taskAuthorizers = _publishEndpoint.Publish<NotifyUids>(new
            {
                message.CorrelationId,
                newRecord.EmpowermentId,
                Uids = newRecord.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }),
                EventCode = Events.EmpowermentCreated.Code,
                Events.EmpowermentCreated.Translations
            });
            messageTasks.Add(taskAuthorizers);
        }

        await Task.WhenAll(messageTasks);

        //Result
        return Ok(newRecordsData.Select(r => r.EmpowermentId));
    }

    public async Task<ServiceResult<bool>> ChangeEmpowermentStatusAsync(ChangeEmpowermentStatus message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var logEventCode = message.Status switch
        {
            EmpowermentStatementStatus.CollectingAuthorizerSignatures => LogEventCode.EMPOWERMENT_IS_COLLECTED_SIGNATURES,
            EmpowermentStatementStatus.Active => LogEventCode.EMPOWERMENT_IS_ACTIVATED,
            EmpowermentStatementStatus.Denied => LogEventCode.EMPOWERMENT_IS_DENIED,
            EmpowermentStatementStatus.DisagreementDeclared => LogEventCode.EMPOWERMENT_IS_DISAGREEMENT_DECLARED,
            EmpowermentStatementStatus.Withdrawn => LogEventCode.EMPOWERMENT_IS_WITHDRAWN,
            EmpowermentStatementStatus.Unconfirmed => LogEventCode.EMPOWERMENT_IS_UNCONFIRMED,
            _ => LogEventCode.CHANGE_EMPOWERMENT_STATUS
        };
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, JsonConvert.SerializeObject(message) },
            { nameof(message.EmpowermentId), message.EmpowermentId },
            { nameof(message.Status), message.Status.ToString() }
        };
        AddAuditLog(message.CorrelationId, logEventCode, LogEventLifecycle.REQUEST, payload: eventPayload);

        var validator = new ChangeEmpowermentStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(ChangeEmpowermentStatus), validationResult);
            var err = validationResult.Errors.Select(e => new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage));
            eventPayload.Add("Reason", string.Join(",", err));
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(message.CorrelationId, logEventCode, LogEventLifecycle.FAIL, payload: eventPayload);
            return BadRequest<bool>(validationResult.Errors);
        }

        var dbRecord = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
             .Where(es => es.Id == message.EmpowermentId)
             .FirstOrDefaultAsync();

        if (dbRecord is null)
        {
            _logger.LogWarning("{CommandName} failed. Non existing Id {EmpowermentId}", nameof(ChangeEmpowermentStatus), message.EmpowermentId);
            eventPayload.Add("Reason", "No empowerment with such id.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.NotFound);
            AddAuditLog(message.CorrelationId, logEventCode, LogEventLifecycle.FAIL, payload: eventPayload);
            return NotFound<bool>(nameof(EmpowermentStatement.Id), message.EmpowermentId);
        }

        var targets = dbRecord.AuthorizerUids.Cast<UidResult>().Union(dbRecord.EmpoweredUids.Cast<UidResult>());
        foreach (UidResult target in targets)
        {
            var currentPayload = new SortedDictionary<string, object>(eventPayload)
            {
                [AuditLoggingKeys.TargetUid] = target.Uid,
                [AuditLoggingKeys.TargetUidType] = target.UidType.ToString(),
                [AuditLoggingKeys.TargetName] = target.Name
            };
            AddAuditLog(message.CorrelationId, logEventCode, suffix: LogEventLifecycle.REQUEST, payload: currentPayload);
        }

        dbRecord.Status = message.Status;

        if (message.Status == EmpowermentStatementStatus.Denied && message.DenialReason != EmpowermentsDenialReason.None)
        {
            dbRecord.DenialReason = message.DenialReason;
            eventPayload.Add(nameof(message.DenialReason), message.DenialReason.ToString());
        }

        var newStatusHistoryRecord = new StatusHistoryRecord
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            EmpowermentStatement = dbRecord,
            Status = message.Status
        };

        //Before setting active state of Legal Entity empowerment, we have to ensure way of representation is either Severally ot Jointly. 
        //In case it is OtherWay we have to set the final status UnConfirmed instead of Active
        if (dbRecord.OnBehalfOf == OnBehalfOf.LegalEntity && message.Status == EmpowermentStatementStatus.Active)
        {
            var legalEntityActualStateResponse = await _verificationService.GetLegalEntityActualStateAsync(message.CorrelationId, dbRecord.Uid);
            var wOr = legalEntityActualStateResponse?.Result?.GetWayOfRepresentation();

            if (wOr?.OtherWay ?? true) // If we were unable to parse the way of representation we go unclear
            {
                newStatusHistoryRecord.Status = EmpowermentStatementStatus.Unconfirmed;
                dbRecord.Status = EmpowermentStatementStatus.Unconfirmed;
            }
        }

        await _context.EmpowermentStatusHistory.AddAsync(newStatusHistoryRecord);
        await _context.SaveChangesAsync();
        foreach (UidResult target in targets)
        {
            var currentPayload = new SortedDictionary<string, object>(eventPayload)
            {
                [AuditLoggingKeys.TargetUid] = target.Uid,
                [AuditLoggingKeys.TargetUidType] = target.UidType.ToString(),
                [AuditLoggingKeys.TargetName] = target.Name
            };
            AddAuditLog(message.CorrelationId, logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: currentPayload);
        }
        _logger.LogInformation("Changed empowerment statement {EmpowermentStatementId} status to {NewStatus}", message.EmpowermentId, message.Status);
        return Ok(true);
    }

    public async Task<ServiceResult<IPaginatedData<EmpowermentStatementResult>>> GetEmpowermentsToMeByFilterAsync(GetEmpowermentsToMeByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentsToMeByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsToMeByFilter), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementResult>>(validationResult.Errors);
        }

        // Action
        var disallowedStatuses = new List<EmpowermentStatementStatus>
        {
            EmpowermentStatementStatus.Created,
            EmpowermentStatementStatus.CollectingAuthorizerSignatures,
            EmpowermentStatementStatus.Denied
        };

        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Include(es => es.EmpowermentDisagreements)
           .Include(es => es.EmpowermentWithdrawals
                   .Where(ew => ew.Status == EmpowermentWithdrawalStatus.Completed)
                   .OrderByDescending(ew => ew.ActiveDateTime)
                   .Take(1))
           .Include(es => es.StatusHistory
                   .OrderByDescending(sh => sh.DateTime))
           .Where(es => (
                            !disallowedStatuses.Contains(es.Status)
                            || es.StatusHistory.Any(shi => shi.Status == EmpowermentStatementStatus.Active || shi.Status == EmpowermentStatementStatus.Unconfirmed) // Include it if it was Active or Uncofirmed at some point regardless of it's current status
                        )
                        && es.EmpoweredUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
           .AsNoTracking()
           .AsSingleQuery();

        if (!string.IsNullOrWhiteSpace(message.Number))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Number.Contains(message.Number.ToUpper()));
        }

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsToMeFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
                EmpowermentsToMeFilterStatus.Denied => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Denied),
                EmpowermentsToMeFilterStatus.DisagreementDeclared => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.DisagreementDeclared),
                EmpowermentsToMeFilterStatus.Withdrawn => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Withdrawn),
                EmpowermentsToMeFilterStatus.Expired => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && es.ExpiryDate < DateTime.UtcNow),
                EmpowermentsToMeFilterStatus.Unconfirmed => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Unconfirmed),
                _ => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active),
            };
        }

        if (!string.IsNullOrWhiteSpace(message.Authorizer))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Name.ToLower().Contains(message.Authorizer.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ProviderName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.ProviderName.ToLower().Contains(message.ProviderName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ServiceName))
        {
            empowermentStatements = empowermentStatements
                .Where(es => es.ServiceName.ToLower().Contains(message.ServiceName.ToLower()) || es.ServiceId.ToString().ToLower().Contains(message.ServiceName.ToLower()));
        }

        if (message.ValidToDate.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate != null && es.ExpiryDate.Value <= message.ValidToDate.Value.ToUniversalTime());
        }

        if (message.ShowOnlyNoExpiryDate.HasValue && message.ShowOnlyNoExpiryDate.Value == true)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate == null);
        }

        if (message.OnBehalfOf.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.OnBehalfOf == message.OnBehalfOf.Value);

            if (message.OnBehalfOf.Value == OnBehalfOf.LegalEntity && !string.IsNullOrWhiteSpace(message.Eik))
            {
                empowermentStatements = empowermentStatements.Where(es => es.UidType == IdentifierType.NotSpecified && es.Uid == message.Eik);
            }
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsToMeSortBy.Id => empowermentStatements.OrderByDescending(es => es.Id),
                    EmpowermentsToMeSortBy.Authorizer => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsToMeSortBy.ProviderName => empowermentStatements.OrderByDescending(es => es.ProviderName),
                    EmpowermentsToMeSortBy.ServiceName => empowermentStatements.OrderByDescending(es => es.ServiceName),

                    EmpowermentsToMeSortBy.Status => empowermentStatements
                        .OrderByDescending(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenBy(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate < DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenByDescending(es => es.ExpiryDate),

                    EmpowermentsToMeSortBy.CreatedOn => empowermentStatements.OrderByDescending(es => es.CreatedOn),
                    _ => empowermentStatements.OrderByDescending(es => es.Name),
                };
            }
            else
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsToMeSortBy.Id => empowermentStatements.OrderBy(es => es.Id),
                    EmpowermentsToMeSortBy.Authorizer => empowermentStatements.OrderBy(es => es.Name),
                    EmpowermentsToMeSortBy.ProviderName => empowermentStatements.OrderBy(es => es.ProviderName),
                    EmpowermentsToMeSortBy.ServiceName => empowermentStatements.OrderBy(es => es.ServiceName),

                    EmpowermentsToMeSortBy.Status => empowermentStatements
                        .OrderBy(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenBy(es => es.ExpiryDate),

                    EmpowermentsToMeSortBy.CreatedOn => empowermentStatements.OrderBy(es => es.CreatedOn),
                    _ => empowermentStatements.OrderBy(es => es.Name),
                };
            }
        }
        else
        {
            empowermentStatements = empowermentStatements
                .OrderBy(es => es.Status)
                // We need to separate Expired from Active empowerments by ExpiryDate
                // to have them properly ordered on CreatedOn later on.
                // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                // on the other hand when ExpiryDate is not null and in the past - "Expired".
                .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                .ThenByDescending(es => es.CreatedOn)
                .ThenBy(es => es.ExpiryDate);
        }

        // Execute query
        var result = await PaginatedData<EmpowermentStatementResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);

        result.Data.ToList().ForEach(x => x.CalculateStatusOn(DateTime.UtcNow));

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IPaginatedData<EmpowermentStatementFromMeResult>>> GetEmpowermentsFromMeByFilterAsync(GetEmpowermentsFromMeByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentsFromMeByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsFromMeByFilter), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementFromMeResult>>(validationResult.Errors);
        }

        // Action
        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Include(es => es.EmpowermentSignatures)
           .Include(es => es.EmpowermentWithdrawals
               .Where(ew => ew.Status == EmpowermentWithdrawalStatus.Completed || ew.Status == EmpowermentWithdrawalStatus.InProgress)
               .OrderByDescending(ew => ew.StartDateTime)
               .Take(1))
           .Include(es => es.EmpowermentDisagreements)
           .Include(es => es.StatusHistory
                .OrderByDescending(sh => sh.DateTime))
           .Where(es => es.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
           .AsQueryable<EmpowermentStatementFromMeResult>()
           .AsNoTracking()
           .AsSingleQuery();

        if (!string.IsNullOrWhiteSpace(message.Number))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Number.Contains(message.Number.ToUpper()));
        }

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsFromMeFilterStatus.Created => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Created),
                EmpowermentsFromMeFilterStatus.CollectingAuthorizerSignatures => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.CollectingAuthorizerSignatures),
                EmpowermentsFromMeFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
                EmpowermentsFromMeFilterStatus.Denied => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Denied),
                EmpowermentsFromMeFilterStatus.DisagreementDeclared => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.DisagreementDeclared),
                EmpowermentsFromMeFilterStatus.Withdrawn => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Withdrawn),
                EmpowermentsFromMeFilterStatus.Expired => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && es.ExpiryDate < DateTime.UtcNow),
                EmpowermentsFromMeFilterStatus.Unconfirmed => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Unconfirmed),
                _ => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active),
            };
        }

        if (message.OnBehalfOf.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.OnBehalfOf == message.OnBehalfOf.Value);

            if (message.OnBehalfOf.Value == OnBehalfOf.LegalEntity && !string.IsNullOrWhiteSpace(message.Eik))
            {
                empowermentStatements = empowermentStatements.Where(es => es.UidType == IdentifierType.NotSpecified && es.Uid == message.Eik);
            }
        }

        if (!string.IsNullOrWhiteSpace(message.Authorizer))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Name.ToLower().Contains(message.Authorizer.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ProviderName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.ProviderName.ToLower().Contains(message.ProviderName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ServiceName))
        {
            empowermentStatements = empowermentStatements
                .Where(es => es.ServiceName.ToLower().Contains(message.ServiceName.ToLower()) || es.ServiceId.ToString().ToLower().Contains(message.ServiceName.ToLower()));
        }

        if (message.ValidToDate.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate != null && es.ExpiryDate.Value <= message.ValidToDate.Value.ToUniversalTime());
        }

        if (message.ShowOnlyNoExpiryDate.HasValue && message.ShowOnlyNoExpiryDate.Value == true)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate == null);
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsFromMeSortBy.Name => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsFromMeSortBy.ProviderName => empowermentStatements.OrderByDescending(es => es.ProviderName),
                    EmpowermentsFromMeSortBy.ServiceName => empowermentStatements.OrderByDescending(es => es.ServiceName),
                    EmpowermentsFromMeSortBy.Uid => empowermentStatements.OrderByDescending(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsFromMeSortBy.Status => empowermentStatements
                        .OrderByDescending(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenBy(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate < DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenByDescending(es => es.ExpiryDate),

                    EmpowermentsFromMeSortBy.CreatedOn => empowermentStatements.OrderByDescending(es => es.CreatedOn),
                    _ => empowermentStatements.OrderByDescending(es => es.Name),
                };
            }
            else
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsFromMeSortBy.Name => empowermentStatements.OrderBy(es => es.Name),
                    EmpowermentsFromMeSortBy.ProviderName => empowermentStatements.OrderBy(es => es.ProviderName),
                    EmpowermentsFromMeSortBy.ServiceName => empowermentStatements.OrderBy(es => es.ServiceName),
                    EmpowermentsFromMeSortBy.Uid => empowermentStatements.OrderBy(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsFromMeSortBy.Status => empowermentStatements
                        .OrderBy(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenBy(es => es.ExpiryDate),

                    EmpowermentsFromMeSortBy.CreatedOn => empowermentStatements.OrderBy(es => es.CreatedOn),
                    _ => empowermentStatements.OrderBy(es => es.Name),
                };
            }
        }
        else
        {
            empowermentStatements = empowermentStatements
                .OrderBy(es => es.Status)
                // We need to separate Expired from Active empowerments by ExpiryDate
                // to have them properly ordered on CreatedOn later on.
                // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                // on the other hand when ExpiryDate is not null and in the past - "Expired".
                .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                .ThenByDescending(es => es.CreatedOn)
                .ThenBy(es => es.ExpiryDate);
        }
        if (message.EmpoweredUids is not null && message.EmpoweredUids.Any())
        {
            var ids = _context.EmpoweredUids
                    //.Where(eu => message.EmpoweredUids.Any(m => eu.Uid == m.Uid && eu.UidType == m.UidType)) // Cannot translate to SQL
                    .Where(eu => message.EmpoweredUids.Select(m => m.Uid).Contains(eu.Uid))
                    .GroupBy(gr => gr.EmpowermentStatementId)
                    .ToList() // Materialize the query
                    .Where(group => message.EmpoweredUids.All(m => group.Any(gr => gr.Uid == m.Uid && gr.UidType == m.UidType))) // Get empowerments that have the filtered Uids
                    .Select(x => x.Key) // Select EmpowermentIds
                    .ToHashSet(); // Distinct
            empowermentStatements = empowermentStatements.Where(x => ids.Contains(x.Id));
        }

        // Execute query
        var result = await PaginatedData<EmpowermentStatementFromMeResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);

        result.Data.ToList().ForEach(x => x.CalculateStatusOn(DateTime.UtcNow));

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IEnumerable<EmpowermentWithdrawalReasonResult>>> GetEmpowermentWithdrawReasonsAsync(GetEmpowermentWithdrawReasons message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentWithdrawReasonsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentWithdrawReasons), validationResult);
            return BadRequest<IEnumerable<EmpowermentWithdrawalReasonResult>>(validationResult.Errors);
        }

        // Execute
        IEnumerable<EmpowermentWithdrawalReasonResult> result = await _context.EmpowermentWithdrawalReasons.ToArrayAsync();

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<string>> WithdrawEmpowermentAsync(WithdrawEmpowerment message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new WithdrawEmpowermentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentWithdrawReasons), validationResult);
            return BadRequest<string>(validationResult.Errors);
        }

        var empowerment = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
            .Include(es => es.EmpowermentWithdrawals
                .OrderByDescending(ew => ew.StartDateTime)
                .Take(1))
            .AsSplitQuery()
            .FirstOrDefaultAsync(es => es.Id == message.EmpowermentId);

        if (empowerment == null)
        {
            _logger.LogWarning("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound<string>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        // Check if the withdraw person is an authorizer
        if (!empowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
            _logger.LogWarning("{Uid} can't be allowed to withdraw empowerment with id {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.Uid}' can't be allowed to withdraw this empowerment.")
                }
            };
        }

        // Only Active empowerment can be withdraw
        var withdrawableStatuses = new EmpowermentStatementStatus[]
        {
            EmpowermentStatementStatus.CollectingAuthorizerSignatures,
            EmpowermentStatementStatus.Active,
            EmpowermentStatementStatus.Unconfirmed
        };
        if (!withdrawableStatuses.Contains(empowerment.Status))
        {
            _logger.LogWarning("Can't withdraw empowerment with id {EmpowermentId} and status {CurrentStatus}.", message.EmpowermentId, empowerment.Status);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Invalid status", $"'Only {string.Join(",", withdrawableStatuses.Select(s => s.ToString()))} empowerments can be withdrawn.")
                }
            };
        }

        // When starting new withdrawal process reason is mandatory
        if (string.IsNullOrWhiteSpace(message.Reason))
        {
            _logger.LogWarning("Can't withdraw empowerment without a reason.");
            return BadRequest<string>(nameof(message.Reason), "Must not be empty");
        }

        // Start process
        var empowermentWithdrawId = Guid.NewGuid();
        _context.EmpowermentWithdrawals.Add(new EmpowermentWithdrawal
        {
            Id = empowermentWithdrawId,
            StartDateTime = DateTime.UtcNow,
            IssuerUid = message.Uid,
            IssuerUidType = message.UidType,
            Reason = message.Reason,
            Status = EmpowermentWithdrawalStatus.InProgress,
            EmpowermentStatement = empowerment
        });

        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish<InitiateEmpowermentWithdrawalProcess>(new
        {
            message.CorrelationId,
            WithdrawalsCollectionsDeadline = empowerment.ExpiryDate,
            IssuerUid = message.Uid,
            IssuerUidType = message.UidType,
            message.Reason,
            message.EmpowermentId,
            empowerment.OnBehalfOf,
            AuthorizerUids = empowerment.AuthorizerUids.Select(au => new UserIdentifierData { Uid = au.Uid, UidType = au.UidType }),
            EmpoweredUids = empowerment.EmpoweredUids.Select(eu => new UserIdentifierData { Uid = eu.Uid, UidType = eu.UidType }),
            EmpowermentWithdrawalId = empowermentWithdrawId,
            LegalEntityUid = empowerment.Uid,
            LegalEntityName = empowerment.Name,
            IssuerName = message.Name,
            empowerment.IssuerPosition
        });

        _logger.LogInformation("Confirmed withdrawal of empowerment with id {EmpowermentId}.", message.EmpowermentId);

        return Accepted(empowerment.Number);
    }

    public async Task<ServiceResult<bool>> ChangeEmpowermentsWithdrawStatusAsync(ChangeEmpowermentWithdrawalStatus message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var validator = new ChangeEmpowermentsWithdrawStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ChangeEmpowermentWithdrawalStatus), validationResult);

            return BadRequest<bool>(validationResult.Errors);
        }

        var dbRecord = await _context.EmpowermentWithdrawals
            .Where(es => es.Id == message.EmpowermentWithdrawalId)
            .FirstOrDefaultAsync();

        if (dbRecord is null)
        {
            _logger.LogInformation("{CommandName} failed. Non existing Id {EmpowermentWithdrawId}",
                nameof(ChangeEmpowermentWithdrawalStatus), message.EmpowermentWithdrawalId);

            return NotFound<bool>(nameof(EmpowermentWithdrawal.Id), message.EmpowermentWithdrawalId);
        }

        dbRecord.Status = message.Status;
        if (message.Status == EmpowermentWithdrawalStatus.Completed)
        {
            dbRecord.ActiveDateTime = DateTime.UtcNow;
        }
        _context.SaveChanges();
        _logger.LogInformation("Changed empowerment withdraw {EmpowermentWithdrawId} status to {NewStatus}",
            message.EmpowermentWithdrawalId, message.Status);

        return Ok(true);
    }

    public async Task<ServiceResult<IEnumerable<EmpowermentDisagreementReasonResult>>> GetEmpowermentDisagreementReasonsAsync(GetEmpowermentDisagreementReasons message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentDisagreementReasonsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentDisagreementReasons), validationResult);
            return BadRequest<IEnumerable<EmpowermentDisagreementReasonResult>>(validationResult.Errors);
        }

        // Execute
        IEnumerable<EmpowermentDisagreementReasonResult> result = await _context.EmpowermentDisagreementReasons.ToArrayAsync();

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<string>> DisagreeEmpowermentAsync(DisagreeEmpowerment message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DisagreeEmpowermentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(DisagreeEmpowerment), validationResult);
            return BadRequest<string>(validationResult.Errors);
        }

        var empowerment = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
            .AsSplitQuery()
            .FirstOrDefaultAsync(es => es.Id == message.EmpowermentId);

        if (empowerment == null)
        {
            _logger.LogInformation("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound<string>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        // Check if person declaring disagreement is empowered
        if (!empowerment.EmpoweredUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            _logger.LogWarning("{Uid} forbidden to declare disagreement with empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.Uid}' can't be allowed to disagree this empowerment.")
                }
            };
        }

        // Only empowerment with these statuses can be disagreed
        var disagreeableStatuses = new EmpowermentStatementStatus[] { EmpowermentStatementStatus.Active, EmpowermentStatementStatus.Unconfirmed };
        if (!disagreeableStatuses.Contains(empowerment.Status)
            || (empowerment.ExpiryDate.HasValue && empowerment.ExpiryDate.Value < DateTime.UtcNow))
        {
            _logger.LogWarning("{Uid} tried declaring disagreement with expired empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Invalid status", $"'Only {string.Join(",", disagreeableStatuses.Select(s => s.ToString()))} empowerments can be disagreed.")
                }
            };
        }

        var checkUidForRestrictionResult = await _checkUidsRestrictionsClient.GetResponse<ServiceResult<UidsRestrictionsResult>>(new
        {
            message.CorrelationId,
            message.EmpowermentId,
            Uids = new UserIdentifierData[] { new() { Uid = message.Uid, UidType = message.UidType, Name = message.Name } },
            RespondWithRawServiceResult = true,
            RapidRetries = true
        });

        if (checkUidForRestrictionResult == null)
        {
            _logger.LogWarning("No response from restriction check for {Uid} during declaring disagreement with empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = "Failed checking uid restriction status."
            };
        }

        if (checkUidForRestrictionResult.Message?.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Failed restriction check for {Uid} during declaring disagreement with empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);

            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = string.Join(",", checkUidForRestrictionResult.Message?.Errors?.Select(kvp => kvp.Value) ?? Array.Empty<string>())
            };
        }

        if (!checkUidForRestrictionResult.Message?.Result?.Successful ?? true)
        {
            _logger.LogWarning("{Uid} has restrictions. Denying declaring disagreement with empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);

            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"After checking '{message.Uid}' it can't be allowed to disagree this empowerment.")
                }
            };
        }

        var empowermentDisagreement = new EmpowermentDisagreement
        {
            Id = Guid.NewGuid(),
            ActiveDateTime = DateTime.UtcNow,
            IssuerUid = message.Uid,
            IssuerUidType = message.UidType,
            Reason = message.Reason,
            EmpowermentStatement = empowerment
        };

        HttpResponseMessage response;
        try
        {
            response = await TimestampPlainTextDataAsync(message.CorrelationId, empowermentDisagreement.Id, empowermentDisagreement.ToString());
        }
        catch (Exception ex)
        {
            var logMessage = "An unexpected error occurred during the timestamping server call.";
            _logger.LogError(ex, logMessage);
            return InternalServerError<string>(logMessage);
        }
        var responseRawData = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogInformation("Deserialization of timestamp token response failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }

        var signature = responseObj?.Data?.Signatures?.FirstOrDefault()?.Signature;
        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogWarning("Malformed or missing signature data while disagreeing empowerment {EmpowermentId} with disagreement {EmpowermentDisagreementId}", empowerment.Id, empowermentDisagreement.Id);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }
        empowermentDisagreement.TimestampData = signature;

        _context.EmpowermentDisagreements.Add(empowermentDisagreement);
        empowerment.Status = EmpowermentStatementStatus.DisagreementDeclared;

        await _context.SaveChangesAsync();

        var messageTasks = new List<Task>
        {
            // It is needed to publish ChangeEmpowermentStatus command, because it also insert status history record.
            _publishEndpoint.Publish<ChangeEmpowermentStatus>(new
            {
                message.CorrelationId,
                message.EmpowermentId,
                Status = EmpowermentStatementStatus.DisagreementDeclared,
                DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
            }),

            // Notify all
            _publishEndpoint.Publish<NotifyUids>(new
            {
                message.CorrelationId,
                message.EmpowermentId,
                Uids = empowerment.AuthorizerUids.Select(au => new UserIdentifierData { Uid = au.Uid, UidType = au.UidType, Name = au.Name }),
                EventCode = Events.EmpowermentWasDisagreed.Code,
                Events.EmpowermentWasDisagreed.Translations
            }),
            _publishEndpoint.Publish<NotifyUids>(new
            {
                message.CorrelationId,
                message.EmpowermentId,
                Uids = empowerment.EmpoweredUids.Select(eu => new UserIdentifierData { Uid = eu.Uid, UidType = eu.UidType, Name = eu.Name }),
                EventCode = Events.EmpowermentToMeWasDisagreed.Code,
                Events.EmpowermentToMeWasDisagreed.Translations
            })
        };

        await Task.WhenAll(messageTasks);
        _logger.LogInformation("{Uid} successfully declared disagreement with empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
        return Ok(empowerment.Number);
    }

    public async Task<ServiceResult<IPaginatedData<EmpowermentStatementResult>>> GetEmpowermentsByDeauAsync(GetEmpowermentsByDeau message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentsByDeauValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsByDeau), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementResult>>(validationResult.Errors);
        }

        // Action
        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Include(es => es.EmpowermentWithdrawals
                   .Where(ew => ew.Status == EmpowermentWithdrawalStatus.Completed)
                   .OrderByDescending(ew => ew.ActiveDateTime)
                   .Take(1))
           .Include(es => es.EmpowermentDisagreements)
           .Include(sh => sh.StatusHistory
                .OrderByDescending(sh => sh.DateTime))
           .Where(es =>
                es.EmpoweredUids.Any(eu => eu.Uid == message.EmpoweredUid && eu.UidType == message.EmpoweredUidType) &&
                es.CreatedOn.Value.Date <= message.StatusOn.ToUniversalTime().Date &&
                es.ProviderId == message.ProviderId &&
                es.ServiceId == message.ServiceId)
           .AsQueryable<EmpowermentStatementResult>()
           .AsNoTracking()
           .AsSingleQuery();

        if (message.OnBehalfOf == OnBehalfOf.Empty)
        {
            empowermentStatements = empowermentStatements
                .Where(es =>
                    es.AuthorizerUids.Any(au => au.Uid == message.AuthorizerUid && au.UidType == message.AuthorizerUidType)
                    || es.Uid == message.AuthorizerUid);
        }

        if (message.OnBehalfOf == OnBehalfOf.Individual)
        {
            empowermentStatements = empowermentStatements
                .Where(es =>
                    es.OnBehalfOf == message.OnBehalfOf &&
                    es.AuthorizerUids.Any(au => au.Uid == message.AuthorizerUid && au.UidType == message.AuthorizerUidType));
        }

        if (message.OnBehalfOf == OnBehalfOf.LegalEntity)
        {
            empowermentStatements = empowermentStatements
                .Where(es =>
                    es.OnBehalfOf == message.OnBehalfOf &&
                    (es.Uid == message.AuthorizerUid ||
                        es.AuthorizerUids.Any(au => au.Uid == message.AuthorizerUid && au.UidType == message.AuthorizerUidType)));
        }

        if (message.VolumeOfRepresentation is not null)
        {
            var ids = (await empowermentStatements
                   .ToListAsync()).Where(es => message.VolumeOfRepresentation.All(vor => es.VolumeOfRepresentation.Select(x => x.Name).Contains(vor)))
                   .Select(e => e.Id);

            empowermentStatements = empowermentStatements.Where(x => ids.Contains(x.Id));
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsByDeauSortBy.Name => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsByDeauSortBy.ServiceName => empowermentStatements.OrderByDescending(es => es.ServiceName),
                    EmpowermentsByDeauSortBy.ExpiryDate => empowermentStatements.OrderByDescending(es => es.ExpiryDate),
                    _ => empowermentStatements.OrderByDescending(es => es.Name),
                };
            }
            else
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsByDeauSortBy.Name => empowermentStatements.OrderBy(es => es.Name),
                    EmpowermentsByDeauSortBy.ServiceName => empowermentStatements.OrderBy(es => es.ServiceName),
                    EmpowermentsByDeauSortBy.ExpiryDate => empowermentStatements.OrderBy(es => es.ExpiryDate),
                    _ => empowermentStatements.OrderBy(es => es.Name),
                };
            }
        }
        else
        {
            empowermentStatements = empowermentStatements
                .OrderBy(es => es.Status)
                .ThenByDescending(es => !es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow))
                .ThenBy(es => es.ExpiryDate);
        }

        // Execute query 
        var result = await PaginatedData<EmpowermentStatementResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);
        var activeOrUnconfirmedEmpowermentStatements = result.Data.Where(es => es.Status == EmpowermentStatementStatus.Active || es.Status == EmpowermentStatementStatus.Unconfirmed);

        if (activeOrUnconfirmedEmpowermentStatements.Any())
        {
            var sagasCheckResult = await _checkEmpowermentVerificationSagasClient.GetResponse<EmpowermentsVerificationSagasCheckResult>(new
            {
                message.CorrelationId,
                EmpowermentIds = activeOrUnconfirmedEmpowermentStatements.Select(x => x.Id)
            });
            if (!sagasCheckResult.Message.AllSagasExistAndFinished)
            {
                var tasks = activeOrUnconfirmedEmpowermentStatements
                            .Where(es => sagasCheckResult.Message.MissingIds.Contains(es.Id))
                            .Select(es => _publishEndpoint.Publish<InitiateEmpowermentValidationProcess>(new
                            {
                                message.CorrelationId,
                                EmpowermentId = es.Id,
                                es.OnBehalfOf,
                                es.Uid,
                                es.UidType,
                                es.Name,
                                es.IssuerPosition,
                                es.AuthorizerUids,
                                es.EmpoweredUids
                            }));

                await Task.WhenAll(tasks);
                return Accepted(PaginatedData<EmpowermentStatementResult>.CreateEmpty(message.PageIndex));
            }
        }

        result.Data.ToList().ForEach(x => x.CalculateStatusOn(message.StatusOn));

        // Send EmpowermentIsBeingCheckedFromDEAU notifications to all involved Uids
        var notifiedUids = result.Data.SelectMany(s => s.AuthorizerUids).Select(u => (u.Uid, u.UidType)).ToList();
        notifiedUids.AddRange(result.Data.SelectMany(s => s.EmpoweredUids).Select(u => (u.Uid, u.UidType)).ToList());
        notifiedUids.AddRange(result.Data.Where(es => es.OnBehalfOf == OnBehalfOf.Individual).Select(u => (u.Uid, u.UidType)).ToList());
        notifiedUids = notifiedUids.Distinct().ToList();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(async () =>
        {
            var tasks = notifiedUids.Select(n => _notificationSender.SendAsync(
                new NotifyUid
                {
                    CorrelationId = message.CorrelationId,
                    Uid = n.Uid,
                    UidType = n.UidType,
                    EventCode = Events.EmpowermentIsBeingCheckedByDEAU.Code
                }));
            await Task.WhenAll(tasks);
        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<Guid>> DenyEmpowermentByDeauAsync(DenyEmpowermentByDeau message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DenyEmpowermentByDeauValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(DenyEmpowermentByDeau), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var empowermentStatement = await _context.EmpowermentStatements.FirstOrDefaultAsync(s => s.Id == message.EmpowermentId);
        if (empowermentStatement is null)
        {
            _logger.LogInformation("Provider {ProviderId} tried to deny non-existent empowerment ({EmpowermentId}).", message.ProviderId, message.EmpowermentId);
            return NotFound<Guid>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        //Check if the empowerment with the given Id belongs to the logged DEAU
        if (empowermentStatement.ProviderId != message.ProviderId)
        {
            _logger.LogInformation("Provider {ProviderId} tried to deny another supplier's empowerment ({EmpowermentId}).", message.ProviderId, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.ProviderId}' can't be allowed to deny this empowerment.")
                }
            };
        }

        //Check if the empowerment is in status Unconfirmed or Active
        if (empowermentStatement.Status != EmpowermentStatementStatus.Unconfirmed && empowermentStatement.Status != EmpowermentStatementStatus.Active)
        {
            _logger.LogInformation("Provider {ProviderId} tried to deny {EmpowermentStatus} empowerment ({EmpowermentId}).", message.ProviderId, empowermentStatement.Status, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new (nameof(EmpowermentStatement.Status), $"'{empowermentStatement.Status}' empowerments can't be denied.")
                }
            };
        }

        // Action
        empowermentStatement.Status = EmpowermentStatementStatus.Denied;
        empowermentStatement.DenialReason = EmpowermentsDenialReason.DeniedByDeauAdministrator;
        empowermentStatement.DenialReasonComment = message.DenialReasonComment;

        //Create new StatusHistoryRecord & add it to EmpowermentStatusHistory
        var newStatusHistoryRecord = new StatusHistoryRecord
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            EmpowermentStatement = empowermentStatement,
            Status = empowermentStatement.Status
        };
        await _context.EmpowermentStatusHistory.AddAsync(newStatusHistoryRecord);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Changed empowerment statement {EmpowermentStatementId} status to {empowermentStatements.Status}", message.EmpowermentId, empowermentStatement.Status);

        // Notify Authorizers
        await _publishEndpoint.Publish<NotifyUids>(new
        {
            message.CorrelationId,
            message.EmpowermentId,
            Uids = empowermentStatement.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }),
            EventCode = Events.EmpowermentDeclined.Code,
            Events.EmpowermentDeclined.Translations
        });

        // Result
        return Ok(empowermentStatement.Id);
    }

    public async Task<ServiceResult<Guid>> ApproveEmpowermentByDeauAsync(ApproveEmpowermentByDeau message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ApproveEmpowermentByDeauValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ApproveEmpowermentByDeau), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        var empowermentStatement = await _context.EmpowermentStatements.FirstOrDefaultAsync(s => s.Id == message.EmpowermentId);
        if (empowermentStatement is null)
        {
            _logger.LogInformation("Provider {ProviderId} tried to approve non-existent empowerment ({EmpowermentId}).", message.ProviderId, message.EmpowermentId);
            return NotFound<Guid>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        //Check if the empowerment with the given Id belongs to the logged DEAU
        if (empowermentStatement.ProviderId != message.ProviderId)
        {
            _logger.LogInformation("Provider {ProviderId} tried to approve another provider's empowerment ({EmpowermentId}).", message.ProviderId, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.ProviderId}' can't be allowed to approve this empowerment.")
                }
            };
        }

        //Check if the empowerment is in status Unconfirmed 
        if (empowermentStatement.Status != EmpowermentStatementStatus.Unconfirmed)
        {
            _logger.LogInformation("Provider {ProviderId} tried to approve {EmpowermentStatus} empowerment ({EmpowermentId}).", message.ProviderId, empowermentStatement.Status, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new (nameof(EmpowermentStatement.Status), $"'{empowermentStatement.Status}' empowerments can't be approved.")
                }
            };
        }

        // Action
        empowermentStatement.Status = EmpowermentStatementStatus.Active;

        //Create new StatusHistoryRecord & add it to EmpowermentStatusHistory
        var newStatusHistoryRecord = new StatusHistoryRecord
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            EmpowermentStatement = empowermentStatement,
            Status = empowermentStatement.Status
        };
        await _context.EmpowermentStatusHistory.AddAsync(newStatusHistoryRecord);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Changed empowerment statement {EmpowermentStatementId} status to {empowermentStatements.Status}", message.EmpowermentId, empowermentStatement.Status);

        // Notify Authorizers
        await _publishEndpoint.Publish<NotifyUids>(new
        {
            message.CorrelationId,
            message.EmpowermentId,
            Uids = empowermentStatement.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }),
            EventCode = Events.EmpowermentCompleted.Code,
            Events.EmpowermentCompleted.Translations
        });

        // Result
        return Ok(empowermentStatement.Id);
    }

    public async Task<ServiceResult<string>> SignEmpowermentAsync(SignEmpowerment message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SignEmpowermentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(SignEmpowerment), validationResult);
            return BadRequest<string>(validationResult.Errors);
        }

        var empowerment = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpowermentSignatures)
            .AsSplitQuery()
            .FirstOrDefaultAsync(es => es.Id == message.EmpowermentId);

        if (empowerment == null)
        {
            _logger.LogInformation("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound<string>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        // Check if person signing is authorizer
        var xmlEmpowerment = XMLSerializationHelper.DeserializeEmpowermentStatementItem(empowerment.XMLRepresentation);
        if (!empowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType)
            || !xmlEmpowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            _logger.LogWarning("{Uid} forbidden to sign empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.Uid}' can't be allowed to sign this empowerment.")
                }
            };
        }

        if (empowerment.Status != EmpowermentStatementStatus.CollectingAuthorizerSignatures)
        {
            _logger.LogWarning("{Uid} tried signing {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Invalid status", $"Only {EmpowermentStatementStatus.CollectingAuthorizerSignatures} empowerments can be signed.")
                }
            };
        }

        var verifySignatureServiceResult = await _verificationService.VerifySignatureAsync(message.CorrelationId, empowerment.XMLRepresentation, message.DetachedSignature, message.Uid, message.UidType, message.SignatureProvider);
        if (verifySignatureServiceResult.StatusCode != HttpStatusCode.OK)
        {
            return verifySignatureServiceResult.ToType<string>();
        }

        var alreadySigned = empowerment.EmpowermentSignatures.Any(es => es.SignerUid == message.Uid && es.SignerUidType == message.UidType);
        if (alreadySigned)
        {
            _logger.LogWarning("{Uid} has already signed the Empowerment: {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult<string>
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Invalid status", "You have already signed this empowerment")
                }
            };
        }

        try
        {
            _context.EmpowermentSignatures.Add(new EmpowermentSignature
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                SignerUid = message.Uid,
                SignerUidType = message.UidType,
                Signature = message.DetachedSignature,
                EmpowermentStatement = empowerment
            });
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish<EmpowermentSigned>(new
            {
                message.CorrelationId,
                message.EmpowermentId,
                SignerName = message.Name,
                SignerUid = message.Uid,
                SignerUidType = message.UidType,
            });
            _logger.LogInformation("{Uid} successfully signed empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return Ok(empowerment.Number);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Uid} failed signing empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return UnhandledException<string>();
        }
    }

    public async Task<ServiceResult<IEnumerable<EmpowermentStatementResult>>> GetExpiringEmpowermentsAsync(GetExpiringEmpowermentsRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validation
        var validator = new GetExpiringEmpowermentsRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{RequestName} validation failed. {Errors}", nameof(GetExpiringEmpowermentsRequest), validationResult);
            return BadRequest<IEnumerable<EmpowermentStatementResult>>(validationResult.Errors);
        }

        // Action
        IEnumerable<EmpowermentStatementResult> empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Where(es =>
               es.Status == EmpowermentStatementStatus.Active &&
               es.ExpiryDate.HasValue &&
               es.ExpiryDate > DateTime.UtcNow &&
               DateTime.UtcNow.AddDays(request.DaysUntilExpiration).Date >= es.ExpiryDate.Value.Date
            )
           .AsNoTracking()
           .AsSingleQuery()
           .AsEnumerable<EmpowermentStatementResult>();

        // Result
        return Ok(empowermentStatements);
    }

    public async Task<ServiceResult<IPaginatedData<EmpowermentStatementResult>>> GetEmpowermentsByEikAsync(GetEmpowermentsByEik message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentsByEikValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsByEik), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementResult>>(validationResult.Errors);
        }

        // Action
        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Include(es => es.EmpowermentDisagreements)
           .Include(es => es.EmpowermentWithdrawals)
           .Include(es => es.StatusHistory
                .OrderByDescending(sh => sh.DateTime))
           .Where(es => es.Uid == message.Eik && es.OnBehalfOf == OnBehalfOf.LegalEntity && (es.Status == EmpowermentStatementStatus.Active || es.Status == EmpowermentStatementStatus.Unconfirmed))
           .AsQueryable<EmpowermentStatementResult>()
           .AsNoTracking()
           .AsSingleQuery();

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsByEikFilterStatus.Created => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Created),
                EmpowermentsByEikFilterStatus.CollectingAuthorizerSignatures => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.CollectingAuthorizerSignatures),
                EmpowermentsByEikFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
                EmpowermentsByEikFilterStatus.Denied => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Denied),
                EmpowermentsByEikFilterStatus.DisagreementDeclared => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.DisagreementDeclared),
                EmpowermentsByEikFilterStatus.Withdrawn => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Withdrawn),
                EmpowermentsByEikFilterStatus.Expired => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && es.ExpiryDate < DateTime.UtcNow),
                EmpowermentsByEikFilterStatus.Unconfirmed => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Unconfirmed),
                _ => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active),
            };
        }

        if (!string.IsNullOrWhiteSpace(message.ProviderName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.ProviderName.ToLower().Contains(message.ProviderName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ServiceName))
        {
            empowermentStatements = empowermentStatements
                .Where(es => es.ServiceName.ToLower().Contains(message.ServiceName.ToLower()) || es.ServiceId.ToString().ToLower().Contains(message.ServiceName.ToLower()));
        }

        if (message.ValidToDate.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate != null && es.ExpiryDate.Value <= message.ValidToDate.Value.ToUniversalTime());
        }

        if (message.ShowOnlyNoExpiryDate.HasValue && message.ShowOnlyNoExpiryDate.Value == true)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate == null);
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsByEikSortBy.Name => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsByEikSortBy.ProviderName => empowermentStatements.OrderByDescending(es => es.ProviderName),
                    EmpowermentsByEikSortBy.ServiceName => empowermentStatements.OrderByDescending(es => es.ServiceName),
                    EmpowermentsByEikSortBy.Uid => empowermentStatements.OrderByDescending(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsByEikSortBy.Status => empowermentStatements
                        .OrderByDescending(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenBy(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate < DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenByDescending(es => es.ExpiryDate),

                    EmpowermentsByEikSortBy.CreatedOn => empowermentStatements.OrderByDescending(es => es.CreatedOn),
                    _ => empowermentStatements.OrderByDescending(es => es.Name),
                };
            }
            else
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsByEikSortBy.Name => empowermentStatements.OrderBy(es => es.Name),
                    EmpowermentsByEikSortBy.ProviderName => empowermentStatements.OrderBy(es => es.ProviderName),
                    EmpowermentsByEikSortBy.ServiceName => empowermentStatements.OrderBy(es => es.ServiceName),
                    EmpowermentsByEikSortBy.Uid => empowermentStatements.OrderBy(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsByEikSortBy.Status => empowermentStatements
                        .OrderBy(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenBy(es => es.ExpiryDate),

                    EmpowermentsByEikSortBy.CreatedOn => empowermentStatements.OrderBy(es => es.CreatedOn),
                    _ => empowermentStatements.OrderBy(es => es.Name),
                };
            }
        }
        else
        {
            empowermentStatements = empowermentStatements
                .OrderBy(es => es.Status)
                // We need to separate Expired from Active empowerments by ExpiryDate
                // to have them properly ordered on CreatedOn later on.
                // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                // on the other hand when ExpiryDate is not null and in the past - "Expired".
                .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                .ThenByDescending(es => es.CreatedOn)
                .ThenBy(es => es.ExpiryDate);
        }

        if (message.AuthorizerUids is not null && message.AuthorizerUids.Any())
        {
            var ids = _context.AuthorizerUids
                    //.Where(eu => message.EmpoweredUids.Any(m => eu.Uid == m.Uid && eu.UidType == m.UidType)) // Cannot translate to SQL
                    .Where(eu => message.AuthorizerUids.Select(m => m.Uid).Contains(eu.Uid))
                    .GroupBy(gr => gr.EmpowermentStatementId)
                    .ToList() // Materialize the query
                    .Where(group => message.AuthorizerUids.All(m => group.Any(gr => gr.Uid == m.Uid && gr.UidType == m.UidType))) // Get empowerments that have the filtered Uids
                    .Select(x => x.Key) // Select EmpowermentIds
                    .ToHashSet(); // Distinct
            empowermentStatements = empowermentStatements.Where(x => ids.Contains(x.Id));
        }

        // Execute query
        var result = await PaginatedData<EmpowermentStatementResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);

        //If there are any found empowerments, we have to check if the requester is part of representatives
        if (result.Data.Any())
        {
            result.Data.ToList().ForEach(x => x.CalculateStatusOn(DateTime.UtcNow));

            //Confirm legal entity name and issuer
            var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(message.CorrelationId, message.Eik);
            var legalEntity = legalEntityActualState.Result;
            var incorrectCompanyData = !legalEntity.MatchCompanyData(result.Data.First().Name, message.Eik);
            var issuerIsNotRepresenter = !legalEntity.IsAmongRepresentatives(message.IssuerUid, message.IssuerName);
            var representativesDataIsAvailable = legalEntity.ContainsRepresentativesData();

            if (incorrectCompanyData || (representativesDataIsAvailable && issuerIsNotRepresenter))
            {
                var bulstatResult = await _verificationService.CheckLegalEntityInBulstatAsync(new CheckLegalEntityInBulstatRequest
                {
                    CorrelationId = message.CorrelationId,
                    Uid = message.Eik,
                    AuthorizerUids = new AuthorizerUidData[] {
                        new AuthorizerUidData {
                            Name = message.IssuerName,
                            Uid = message.IssuerUid,
                            UidType = message.IssuerUidType
                        }
                    }
                });
                if (bulstatResult is null || bulstatResult.StatusCode != HttpStatusCode.OK || bulstatResult.Result is null || !bulstatResult.Result.Successful)
                {
                    return new ServiceResult<IPaginatedData<EmpowermentStatementResult>>
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Errors = new List<KeyValuePair<string, string>>
                    {
                        new ("Forbidden", "You can't be allowed to access this data.")
                    }
                    };
                }
            }
        }

        // Result
        return Ok(result);
    }

    public async Task<bool?> ConfirmAuthorizersInLegalEntityRepresentationAsync(Guid empowermentId, LegalEntityActualState legalEntityActualState)
    {
        if (legalEntityActualState is null)
        {
            throw new ArgumentNullException(nameof(legalEntityActualState));
        }

        if (Guid.Empty == empowermentId)
        {
            throw new ArgumentNullException(nameof(empowermentId));
        }

        var authorizersData = await _context.AuthorizerUids
            .Where(a => a.EmpowermentStatementId == empowermentId)
            .Select(a => new AuthorizerIdentifierData
            {
                Uid = a.Uid,
                UidType = a.UidType,
                Name = a.Name,
            })
            .ToListAsync();

        if (!authorizersData.Any() || legalEntityActualState.ContainsRepresentativesData())
        {
            _logger.LogWarning("Missing Authorizers or Representatives information");
            return null;
        }

        //We have to check if all authorizers are present among Representatives
        return authorizersData.All(authorizer => legalEntityActualState.IsAmongRepresentatives(authorizer.Uid, authorizer.Name));
    }

    public async Task<EmpowermentStatementResult?> GetEmpowermentStatementByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id));
        }

        return await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ServiceResult<EmpowermentStatementWithSignaturesResult>> GetEmpowermentStatementByIdAsync(GetEmpowermentById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentById), validationResult);
            return BadRequest<EmpowermentStatementWithSignaturesResult>(validationResult.Errors);
        }

        var result = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
            .Include(es => es.EmpowermentSignatures)
            .Include(es => es.EmpowermentDisagreements)
            .Include(es => es.EmpowermentWithdrawals)
            .Include(es => es.StatusHistory
                .OrderByDescending(sh => sh.DateTime))
            .FirstOrDefaultAsync(x => x.Id == message.EmpowermentId);
        if (result is null)
        {
            return NotFound<EmpowermentStatementWithSignaturesResult>(nameof(EmpowermentStatement.Id), message.EmpowermentId);
        }

        return Ok<EmpowermentStatementWithSignaturesResult>(result);
    }

    public async Task<ServiceResult<TimestampingOutcome>> TimestampEmpowermentXmlAsync(Guid correlationId, Guid id)
    {
        if (correlationId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(correlationId));
        }
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id));
        }

        var empowermentStatement = await _context.EmpowermentStatements.FirstOrDefaultAsync(es => es.Id == id);
        if (empowermentStatement is null)
        {
            return NotFound<TimestampingOutcome>(nameof(EmpowermentStatement.Id), id);
        }

        if (_context.EmpowermentTimestamps.Any(et => et.EmpowermentStatementId == id))
        {
            _logger.LogWarning("Attempt to timestamp already timestamped Empowerment {EmpowermentId}", id);
            return Conflict<TimestampingOutcome>(nameof(EmpowermentStatement.Timestamp), id);
        }

        var response = await TimestampXMLDataAsync(correlationId, empowermentStatement.Id, empowermentStatement.XMLRepresentation);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<TimestampResponse>(body);
        var signature = result?.Data?.Signatures?.FirstOrDefault()?.Signature;

        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogWarning("Malformed or missing signature for {EmpowermentId}", id);
            return BadGateway<TimestampingOutcome>("Malformed or missing signature.");
        }

        try
        {
            var dbEntity = await _context.EmpowermentTimestamps.AddAsync(new EmpowermentTimestamp
            {
                EmpowermentStatement = empowermentStatement,
                Data = signature,
                DateTime = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException postgresException && postgresException.SqlState == "23505")
            {
                _logger.LogWarning("Attempt to timestamp already timestamped Empowerment {EmpowermentId}", id);
                return Conflict<TimestampingOutcome>(nameof(EmpowermentStatement.Timestamp), id);
            }
            _logger.LogWarning(ex, "DBUpdateException during timestamp saving for Empowerment {EmpowermentId}", id);
            return InternalServerError<TimestampingOutcome>("Persisting timestamp data error.");
        }

        return Ok(new TimestampingOutcome { Successful = true });
    }

    public async Task<ServiceResult<TimestampingOutcome>> TimestampEmpowermentWithdrawalAsync(Guid correlationId, Guid empowermentId, Guid withdrawalId)
    {
        if (empowermentId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(empowermentId));
        }
        if (withdrawalId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(withdrawalId));
        }

        var empowermentWithdrawal = await _context.EmpowermentWithdrawals
                                                    .FirstOrDefaultAsync(ew =>
                                                    ew.Id == withdrawalId
                                                    && ew.EmpowermentStatementId == empowermentId
                                                    && ew.Status == EmpowermentWithdrawalStatus.InProgress);
        if (empowermentWithdrawal is null)
        {
            return NotFound<TimestampingOutcome>(nameof(EmpowermentWithdrawal.Id), withdrawalId);
        }

        if (!string.IsNullOrWhiteSpace(empowermentWithdrawal.TimestampData))
        {
            _logger.LogWarning("Attempt to timestamp already timestamped Empowerment {EmpowermentId}  Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return Conflict<TimestampingOutcome>(nameof(EmpowermentWithdrawal.TimestampData), empowermentId);
        }

        HttpResponseMessage response;
        try
        {
            response = await TimestampPlainTextDataAsync(correlationId, empowermentWithdrawal.Id, empowermentWithdrawal.ToString());
        }
        catch (Exception ex)
        {
            var logMessage = "An unexpected error occurred during the timestamping server call.";
            _logger.LogError(ex, logMessage);
            return InternalServerError<TimestampingOutcome>(logMessage);
        }
        var responseRawData = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogWarning("Deserialization of timestamp token response failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        try
        {
            if (_context.EmpowermentWithdrawals.Any(ew => ew.Id == withdrawalId && ew.EmpowermentStatementId == empowermentId && ew.TimestampData.Length > 0))
            {
                _logger.LogWarning("Attempt to timestamp already timestamped Empowerment {EmpowermentId} Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
                return Conflict<TimestampingOutcome>(nameof(EmpowermentWithdrawal.TimestampData), withdrawalId);
            }
            var signature = responseObj?.Data?.Signatures?.FirstOrDefault()?.Signature;
            if (string.IsNullOrWhiteSpace(signature))
            {
                _logger.LogWarning("Malformed or missing signature data while withdrawing empowerment {EmpowermentId} with Withdrawal {EmpowermentWithdrawalId}", empowermentId, withdrawalId);
                return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
            }

            empowermentWithdrawal.TimestampData = signature;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DBUpdateException during timestamp saving for Empowerment {EmpowermentId} Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return InternalServerError<TimestampingOutcome>("Persisting withdrawal timestamp data error.");
        }

        return Ok(new TimestampingOutcome { Successful = true });
    }

    private async Task<HttpResponseMessage> TimestampXMLDataAsync(Guid correlationId, Guid empowermentStatementId, string xmlRepresentation)
    {
        var bytes = Encoding.UTF8.GetBytes(xmlRepresentation);
        var requestBody = new
        {
            Contents = new[]
            {
                new
                {
                    MediaType = MediaTypeNames.Application.Xml,
                    Data = Convert.ToBase64String(bytes),
                    FileName = $"{empowermentStatementId}.xml",
                    SignatureType = "XADES_BASELINE_B_DETACHED"
                }
            }
        };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, MediaTypeNames.Application.Json);
        return await SendTimestampRequest(correlationId, content);
    }

    private async Task<HttpResponseMessage> TimestampPlainTextDataAsync(Guid correlationId, Guid id, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var requestBody = new
        {
            Contents = new[]
            {
                new
                {
                    MediaType = MediaTypeNames.Text.Plain,
                    Data = Convert.ToBase64String(bytes),
                    FileName = $"{id}.txt",
                    SignatureType = "CADES_BASELINE_B_DETACHED"
                }
            }
        };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, MediaTypeNames.Application.Json);
        return await SendTimestampRequest(correlationId, content);
    }

    private Task<HttpResponseMessage> SendTimestampRequest(Guid correlationId, StringContent content)
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.RequestId, correlationId.ToString());
        return _httpClient.PostAsync(
            "/api/v1/borica/sign",
            content);

    }

    public class TimestampingOutcome
    {
        public bool Successful { get; set; } = false;
    }

    public async Task<IEnumerable<IAuthorizerUidData>> GetAuthorizersByEmpowermentStatementIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id));
        }

        return await _context.AuthorizerUids
            .Where(auid => auid.EmpowermentStatementId == id)
            .Select(auid => new AuthorizerUidData
            {
                Name = auid.Name,
                Uid = auid.Uid,
                UidType = auid.UidType,
            })
            .ToListAsync();
    }


    public async Task<ServiceResult<IPaginatedData<EmpowermentStatementWithSignaturesResult>>> GetEmpowermentsByFilterAsync(GetEmpowermentsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetEmpowermentsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsByFilter), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementWithSignaturesResult>>(validationResult.Errors);
        }

        // Action
        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
           .Include(es => es.EmpowermentSignatures)
           .Include(es => es.EmpowermentWithdrawals
               .Where(ew => ew.Status == EmpowermentWithdrawalStatus.Completed || ew.Status == EmpowermentWithdrawalStatus.InProgress)
               .OrderByDescending(ew => ew.StartDateTime)
               .Take(1))
           .Include(es => es.EmpowermentDisagreements)
           .Include(es => es.StatusHistory
                .OrderByDescending(sh => sh.DateTime))
           .AsQueryable<EmpowermentStatementWithSignaturesResult>()
           .AsNoTracking()
           .AsSingleQuery();

        if (!string.IsNullOrWhiteSpace(message.Number))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Number.Contains(message.Number.ToUpper()));
        }

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsFromMeFilterStatus.Created => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Created),
                EmpowermentsFromMeFilterStatus.CollectingAuthorizerSignatures => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.CollectingAuthorizerSignatures),
                EmpowermentsFromMeFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
                EmpowermentsFromMeFilterStatus.Denied => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Denied),
                EmpowermentsFromMeFilterStatus.DisagreementDeclared => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.DisagreementDeclared),
                EmpowermentsFromMeFilterStatus.Withdrawn => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Withdrawn),
                EmpowermentsFromMeFilterStatus.Expired => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && es.ExpiryDate < DateTime.UtcNow),
                EmpowermentsFromMeFilterStatus.Unconfirmed => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Unconfirmed),
                _ => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active),
            };
        }

        if (message.OnBehalfOf.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.OnBehalfOf == message.OnBehalfOf.Value);
        }

        if (message.CreatedOnFrom.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.CreatedOn >= message.CreatedOnFrom.Value);
        }

        if (message.CreatedOnTo.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.CreatedOn <= message.CreatedOnTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(message.EmpowermentUid))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Uid == message.EmpowermentUid);
        }

        if (!string.IsNullOrWhiteSpace(message.Authorizer))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Name.ToLower().Contains(message.Authorizer.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ProviderName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.ProviderName.ToLower().Contains(message.ProviderName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.ServiceName))
        {
            empowermentStatements = empowermentStatements
                .Where(es => es.ServiceName.ToLower().Contains(message.ServiceName.ToLower()) || es.ServiceId.ToString().ToLower().Contains(message.ServiceName.ToLower()));
        }

        if (message.ValidToDate.HasValue)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate != null && es.ExpiryDate.Value <= message.ValidToDate.Value.ToUniversalTime());
        }

        if (message.ShowOnlyNoExpiryDate.HasValue && message.ShowOnlyNoExpiryDate.Value == true)
        {
            empowermentStatements = empowermentStatements.Where(es => es.ExpiryDate == null);
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsFromMeSortBy.Name => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsFromMeSortBy.ProviderName => empowermentStatements.OrderByDescending(es => es.ProviderName),
                    EmpowermentsFromMeSortBy.ServiceName => empowermentStatements.OrderByDescending(es => es.ServiceName),
                    EmpowermentsFromMeSortBy.Uid => empowermentStatements.OrderByDescending(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsFromMeSortBy.Status => empowermentStatements
                        .OrderByDescending(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenBy(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate < DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenByDescending(es => es.ExpiryDate),

                    EmpowermentsFromMeSortBy.CreatedOn => empowermentStatements.OrderByDescending(es => es.CreatedOn),
                    _ => empowermentStatements.OrderByDescending(es => es.Name),
                };
            }
            else
            {
                empowermentStatements = message.SortBy switch
                {
                    EmpowermentsFromMeSortBy.Name => empowermentStatements.OrderBy(es => es.Name),
                    EmpowermentsFromMeSortBy.ProviderName => empowermentStatements.OrderBy(es => es.ProviderName),
                    EmpowermentsFromMeSortBy.ServiceName => empowermentStatements.OrderBy(es => es.ServiceName),
                    EmpowermentsFromMeSortBy.Uid => empowermentStatements.OrderBy(es => es.EmpoweredUids.Select(e => e.Uid)),

                    EmpowermentsFromMeSortBy.Status => empowermentStatements
                        .OrderBy(es => es.Status)
                        // We need to separate Expired from Active empowerments by ExpiryDate
                        // to have them properly ordered on CreatedOn later on.
                        // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                        // on the other hand when ExpiryDate is not null and in the past - "Expired".
                        .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                        .ThenByDescending(es => es.CreatedOn)
                        .ThenBy(es => es.ExpiryDate),

                    EmpowermentsFromMeSortBy.CreatedOn => empowermentStatements.OrderBy(es => es.CreatedOn),
                    _ => empowermentStatements.OrderBy(es => es.Name),
                };
            }
        }
        else
        {
            empowermentStatements = empowermentStatements
                .OrderBy(es => es.Status)
                // We need to separate Expired from Active empowerments by ExpiryDate
                // to have them properly ordered on CreatedOn later on.
                // When ExpiryDate is null or in the future - that empowerment is considered "Active",
                // on the other hand when ExpiryDate is not null and in the past - "Expired".
                .ThenByDescending(es => es.Status == EmpowermentStatementStatus.Active && (!es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow)))
                .ThenByDescending(es => es.CreatedOn)
                .ThenBy(es => es.ExpiryDate);
        }
        if (message.EmpoweredUids is not null && message.EmpoweredUids.Any())
        {
            var ids = _context.EmpoweredUids
                    //.Where(eu => message.EmpoweredUids.Any(m => eu.Uid == m.Uid && eu.UidType == m.UidType)) // Cannot translate to SQL
                    .Where(eu => message.EmpoweredUids.Select(m => m.Uid).Contains(eu.Uid))
                    .GroupBy(gr => gr.EmpowermentStatementId)
                    .ToList() // Materialize the query
                    .Where(group => message.EmpoweredUids.All(m => group.Any(gr => gr.Uid == m.Uid && gr.UidType == m.UidType))) // Get empowerments that have the filtered Uids
                    .Select(x => x.Key) // Select EmpowermentIds
                    .ToHashSet(); // Distinct
            empowermentStatements = empowermentStatements.Where(x => ids.Contains(x.Id));
        }

        // Execute query
        var result = await PaginatedData<EmpowermentStatementWithSignaturesResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);

        result.Data.ToList().ForEach(x => x.CalculateStatusOn(DateTime.UtcNow));

        // Result
        return Ok(result);
    }


    private async Task<string> BuildNumberAsync(DateTime dateTime)
    {
        var currentNumber = await _numberRegistrator.GetEmpowermentNextNumberAsync(dateTime);

        return $"{NumberPrefix}{currentNumber}/{dateTime:dd.MM.yyyy}";
    }

    private void AddAuditLog(Guid correlationId, LogEventCode logEvent, LogEventLifecycle suffix, string? targetUserId = default, string? message = default, SortedDictionary<string, object>? payload = default)
    {
        if (bool.TryParse(_configuration.GetSection("SkipAuditLogging").Value, out var result) && result)
        {
            return;
        }
        _auditLogger.LogEvent(new AuditLogEvent
        {
            CorrelationId = correlationId.ToString(),
            RequesterSystemId = "RO", // TODO: Change to whatever you see fit
            RequesterUserId = null,
            TargetUserId = targetUserId,
            EventType = $"{logEvent}_{suffix}",
            Message = LogEventMessages.GetLogEventMessage(logEvent, suffix),
            EventPayload = payload
        });
    }

    internal class EmpowermentActivationProcessData : InitiateEmpowermentActivationProcess
    {
        public Guid CorrelationId { get; set; }
        public Guid EmpowermentId { get; set; }
        public string Uid { get; set; }
        public IdentifierType UidType { get; set; }
        public string Name { get; set; }
        public string IssuerPosition { get; set; }
        public string IssuerName { get; set; }
        public OnBehalfOf OnBehalfOf { get; set; }
        public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; }
        public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
