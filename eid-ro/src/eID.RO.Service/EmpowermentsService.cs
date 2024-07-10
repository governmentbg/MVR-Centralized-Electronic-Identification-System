using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Entities;
using eID.RO.Service.EventsRegistration;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Options;
using eID.RO.Service.Requests;
using eID.RO.Service.Responses;
using eID.RO.Service.Validators;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Npgsql;

namespace eID.RO.Service;

public class EmpowermentsService : BaseService
{
    private readonly ILogger<EmpowermentsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRequestClient<CheckUidsRestrictions> _checkUidsRestrictionsClient;
    private readonly IVerificationService _verificationService;
    protected HttpClient _timestampServerHttpClient;
    private readonly TimestampServerOptions _timestampServerOptions;

    private const string _representativeField1Id = "00100";
    private const string _representativeField2Id = "00101";
    private const string _representativeField3Id = "00102";
    private const string _wayOfRepresentationFieldId = "00110";

    public EmpowermentsService(
        ILogger<EmpowermentsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IPublishEndpoint publishEndpoint,
        IRequestClient<CheckUidsRestrictions> checkUidsRestrictionsClient,
        IVerificationService verificationService,
        IHttpClientFactory httpClientFactory,
        IOptions<TimestampServerOptions> timestampServerOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _checkUidsRestrictionsClient = checkUidsRestrictionsClient ?? throw new ArgumentNullException(nameof(checkUidsRestrictionsClient));
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        _timestampServerHttpClient = httpClientFactory.CreateClient(TimestampServerOptions.HTTP_CLIENT_NAME);
        _timestampServerOptions = timestampServerOptions?.Value ?? new TimestampServerOptions();
        _timestampServerOptions.Validate();
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(AddEmpowermentStatement), validationResult);
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
            var empowermentStatement = new EmpowermentStatement
            {
                Id = empowermentId,
                IssuerPosition = issuerPosition,
                CreatedBy = message.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                Uid = message.Uid,
                UidType = message.UidType,
                Name = message.Name,
                OnBehalfOf = message.OnBehalfOf,
                AuthorizerUids = authorizedUids.ToList(),
                EmpoweredUids = message.EmpoweredUids.Select(uid => new EmpoweredUid { Id = Guid.NewGuid(), Uid = uid.Uid, UidType = uid.UidType }).ToList(),
                SupplierId = message.SupplierId,
                SupplierName = message.SupplierName,
                ServiceId = message.ServiceId,
                ServiceName = message.ServiceName,
                VolumeOfRepresentation = message.VolumeOfRepresentation.Select(vor => new VolumeOfRepresentation
                {
                    Code = vor.Code,
                    Name = vor.Name
                }).ToList(),
                StartDate = startDate,
                ExpiryDate = expiryDate,
                XMLRepresentation = XMLSerializationHelper.SerializeEmpowermentStatementItem(
                    new EmpowermentStatementItem
                    {
                        Id = empowermentId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        OnBehalfOf = message.OnBehalfOf.ToString(),
                        Uid = message.Uid,
                        UidType = message.UidType,
                        Name = message.Name,
                        AuthorizerUids = message.AuthorizerUids.Select(x => new UserIdentifierWithNameData { Uid = x.Uid, UidType = x.UidType, Name = x.Name }).ToArray(),
                        EmpoweredUids = message.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }).ToArray(),
                        SupplierId = message.SupplierId,
                        SupplierName = message.SupplierName,
                        ServiceId = message.ServiceId,
                        ServiceName = message.ServiceName,
                        TypeOfEmpowerment = message.TypeOfEmpowerment.ToString(),
                        VolumeOfRepresentation = message.VolumeOfRepresentation.Select(v => new VolumeOfRepresentationItem { Code = v.Code, Name = v.Name }).ToArray(),
                        StartDate = startDate,
                        ExpiryDate = expiryDate,
                    }),
                Status = EmpowermentStatementStatus.Created
            };

            empowermentStatement.StatusHistory.Add(new StatusHistoryRecord
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
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
                AuthorizerUids = message.AuthorizerUids.Select(x => new UserIdentifierWithNameData
                {
                    Uid = x.Uid,
                    UidType = x.UidType,
                    Name = x.Name,
                    IsIssuer = x.IsIssuer
                }),
                EmpoweredUids = empowermentStatement.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
                ExpiryDate = empowermentStatement.ExpiryDate,
            });
        }
        else
        {
            foreach (var empoweredUid in message.EmpoweredUids)
            {
                var empowermentId = Guid.NewGuid();
                var empowermentStatement = new EmpowermentStatement
                {
                    Id = empowermentId,
                    IssuerPosition = issuerPosition,
                    CreatedBy = message.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                    Uid = message.Uid,
                    UidType = message.UidType,
                    Name = message.Name,
                    OnBehalfOf = message.OnBehalfOf,
                    AuthorizerUids = authorizedUids.ToList(),
                    EmpoweredUids = new List<EmpoweredUid> { new() { Id = Guid.NewGuid(), Uid = empoweredUid.Uid, UidType = empoweredUid.UidType } },
                    SupplierId = message.SupplierId,
                    SupplierName = message.SupplierName,
                    ServiceId = message.ServiceId,
                    ServiceName = message.ServiceName,
                    VolumeOfRepresentation = message.VolumeOfRepresentation.Select(vor => new VolumeOfRepresentation
                    {
                        Code = vor.Code,
                        Name = vor.Name
                    }).ToList(),
                    StartDate = startDate,
                    ExpiryDate = expiryDate,
                    XMLRepresentation = XMLSerializationHelper.SerializeEmpowermentStatementItem(
                    new EmpowermentStatementItem
                    {
                        Id = empowermentId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        OnBehalfOf = message.OnBehalfOf.ToString(),
                        Uid = message.Uid,
                        UidType = message.UidType,
                        Name = message.Name,
                        AuthorizerUids = message.AuthorizerUids.Select(x => new UserIdentifierWithNameData
                        {
                            Uid = x.Uid,
                            UidType = x.UidType,
                            Name = x.Name,
                            IsIssuer = x.IsIssuer
                        }).ToArray(),
                        EmpoweredUids = new[] { new UserIdentifierData { Uid = empoweredUid.Uid, UidType = empoweredUid.UidType } },
                        SupplierId = message.SupplierId,
                        SupplierName = message.SupplierName,
                        ServiceId = message.ServiceId,
                        ServiceName = message.ServiceName,
                        TypeOfEmpowerment = message.TypeOfEmpowerment.ToString(),
                        VolumeOfRepresentation = message.VolumeOfRepresentation.Select(v => new VolumeOfRepresentationItem { Code = v.Code, Name = v.Name }).ToArray(),
                        StartDate = startDate,
                        ExpiryDate = expiryDate
                    }),
                    Status = EmpowermentStatementStatus.Created
                };

                empowermentStatement.StatusHistory.Add(new StatusHistoryRecord
                {
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.UtcNow,
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
                    AuthorizerUids = message.AuthorizerUids.Select(x => new UserIdentifierWithNameData
                    {
                        Uid = x.Uid,
                        UidType = x.UidType,
                        Name = x.Name,
                        IsIssuer = x.IsIssuer
                    }),
                    EmpoweredUids = empowermentStatement.EmpoweredUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
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
                Uids = newRecord.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
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

        var validator = new ChangeEmpowermentStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ChangeEmpowermentStatus), validationResult);
            return BadRequest<bool>(validationResult.Errors);
        }

        var dbRecord = await _context.EmpowermentStatements
             .Where(es => es.Id == message.EmpowermentId)
             .FirstOrDefaultAsync();

        if (dbRecord is null)
        {
            _logger.LogInformation("{CommandName} failed. Non existing Id {EmpowermentId}", nameof(ChangeEmpowermentStatus), message.EmpowermentId);
            return NotFound<bool>(nameof(EmpowermentStatement.Id), message.EmpowermentId);
        }

        dbRecord.Status = message.Status;

        if (message.Status == EmpowermentStatementStatus.Denied && message.DenialReason != EmpowermentsDenialReason.None)
        {
            dbRecord.DenialReason = message.DenialReason;
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
            var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(dbRecord.Uid);
            var wOr = GetWayOfRepresentation(legalEntityActualState?.Result);

            if (wOr.OtherWay)
            {
                newStatusHistoryRecord.Status = EmpowermentStatementStatus.Unconfirmed;
                dbRecord.Status = EmpowermentStatementStatus.Unconfirmed;
            }
        }

        await _context.EmpowermentStatusHistory.AddAsync(newStatusHistoryRecord);
        await _context.SaveChangesAsync();

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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsToMeByFilter), validationResult);
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
           .Where(es => !disallowedStatuses.Contains(es.Status) && es.EmpoweredUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
           .AsNoTracking()
           .AsSingleQuery();

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsToMeFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
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

        if (!string.IsNullOrWhiteSpace(message.SupplierName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.SupplierName.ToLower().Contains(message.SupplierName.ToLower()));
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
                    EmpowermentsToMeSortBy.Id => empowermentStatements.OrderByDescending(es => es.Id),
                    EmpowermentsToMeSortBy.Authorizer => empowermentStatements.OrderByDescending(es => es.Name),
                    EmpowermentsToMeSortBy.SupplierName => empowermentStatements.OrderByDescending(es => es.SupplierName),
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
                    EmpowermentsToMeSortBy.SupplierName => empowermentStatements.OrderBy(es => es.SupplierName),
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsFromMeByFilter), validationResult);
            return BadRequest<IPaginatedData<EmpowermentStatementFromMeResult>>(validationResult.Errors);
        }

        // Action
        var empowermentStatements = _context.EmpowermentStatements
           .Include(es => es.AuthorizerUids)
           .Include(es => es.EmpoweredUids)
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

        if (message.Status.HasValue)
        {
            empowermentStatements = message.Status switch
            {
                EmpowermentsFromMeFilterStatus.Created => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Created),
                EmpowermentsFromMeFilterStatus.CollectingAuthorizerSignatures => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.CollectingAuthorizerSignatures),
                EmpowermentsFromMeFilterStatus.Active => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Active && (es.ExpiryDate == null || es.ExpiryDate > DateTime.UtcNow)),
                EmpowermentsFromMeFilterStatus.Denied => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.Denied),
                EmpowermentsFromMeFilterStatus.DisagreementDeclared => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.DisagreementDeclared),
                EmpowermentsFromMeFilterStatus.CollectingWithdrawalSignatures => empowermentStatements.Where(es => es.Status == EmpowermentStatementStatus.CollectingWithdrawalSignatures),
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

        if (!string.IsNullOrWhiteSpace(message.Authorizer))
        {
            empowermentStatements = empowermentStatements.Where(es => es.Name.ToLower().Contains(message.Authorizer.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(message.SupplierName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.SupplierName.ToLower().Contains(message.SupplierName.ToLower()));
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
                    EmpowermentsFromMeSortBy.SupplierName => empowermentStatements.OrderByDescending(es => es.SupplierName),
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
                    EmpowermentsFromMeSortBy.SupplierName => empowermentStatements.OrderBy(es => es.SupplierName),
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentWithdrawReasons), validationResult);
            return BadRequest<IEnumerable<EmpowermentWithdrawalReasonResult>>(validationResult.Errors);
        }

        // Execute
        IEnumerable<EmpowermentWithdrawalReasonResult> result = await _context.EmpowermentWithdrawalReasons.ToArrayAsync();

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult> WithdrawEmpowermentAsync(WithdrawEmpowerment message)
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentWithdrawReasons), validationResult);
            return BadRequest<IEnumerable<EmpowermentWithdrawalReasonResult>>(validationResult.Errors);
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
            _logger.LogInformation("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        // Check if the withdraw person is an authorizer
        if (!empowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            _logger.LogInformation("{Uid} can't be allowed to withdraw empowerment with id {EmpowermentId}.", message.Uid, message.EmpowermentId);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.Uid}' can't be allowed to withdraw this empowerment.")
                }
            };
        }

        // Only Active empowerment can be withdraw
        var withdrawableStatuses = new EmpowermentStatementStatus[] { EmpowermentStatementStatus.Active, EmpowermentStatementStatus.Unconfirmed };
        if (!withdrawableStatuses.Contains(empowerment.Status))
        {
            _logger.LogInformation("Can't withdraw empowerment with id {EmpowermentId} and status {CurrentStatus}.", message.EmpowermentId, empowerment.Status);
            return new ServiceResult<bool>
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
            _logger.LogInformation("Can't withdraw empowerment without a reason.");
            return BadRequest(nameof(message.Reason), "Must not be empty");
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
        return Accepted();
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

    public async Task<ServiceResult> DisagreeEmpowermentAsync(DisagreeEmpowerment message)
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
            return BadRequest(validationResult.Errors);
        }

        var empowerment = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .Include(es => es.EmpoweredUids)
            .AsSplitQuery()
            .FirstOrDefaultAsync(es => es.Id == message.EmpowermentId);

        if (empowerment == null)
        {
            _logger.LogInformation("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        // Check if person declaring disagreement is empowered
        if (!empowerment.EmpoweredUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            _logger.LogInformation("{Uid} forbidden to declare disagreement with empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.Uid}' can't be allowed to disagree this empowerment.")
                }
            };
        }

        // Only Active empowerment can be disagree
        var disagreeableStatuses = new EmpowermentStatementStatus[] { EmpowermentStatementStatus.Active, EmpowermentStatementStatus.Unconfirmed };
        if (!disagreeableStatuses.Contains(empowerment.Status)
            || (empowerment.ExpiryDate.HasValue && empowerment.ExpiryDate.Value < DateTime.UtcNow))
        {
            _logger.LogInformation("{Uid} tried declaring disagreement with expired empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);
            return new ServiceResult
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
            Uids = new UserIdentifierData[] { new() { Uid = message.Uid, UidType = message.UidType } },
            RespondWithRawServiceResult = true,
            RapidRetries = true
        });

        if (checkUidForRestrictionResult == null)
        {
            _logger.LogInformation("No response from restriction check for {Uid} during declaring disagreement with empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = "Failed checking uid restriction status."
            };
        }

        if (checkUidForRestrictionResult.Message?.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Failed restriction check for {Uid} during declaring disagreement with empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);

            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = String.Join(",", checkUidForRestrictionResult.Message?.Errors?.Select(kvp => kvp.Value) ?? Array.Empty<string>())
            };
        }

        if (!checkUidForRestrictionResult.Message?.Result?.Successfull ?? true)
        {
            _logger.LogInformation("{Uid} has restrictions. Denying declaring disagreement with empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);

            return new ServiceResult
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
        using var sha256 = SHA256.Create();
        var disagreementHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(empowermentDisagreement.ToString()));
        var timestampRequest = Rfc3161TimestampRequest.CreateFromData(disagreementHash, HashAlgorithmName.SHA256, requestSignerCertificates: true);

        // I've observed that regardless of hashSize, encoded result was always 59 bytes
        var resultData = new byte[64];
        if (!timestampRequest.TryEncode(resultData, out int OK))
        {
            _logger.LogInformation("Failed encoding result data for empowerment {EmpowermentId}", empowerment.Id);
            return InternalServerError("Failed encoding result data.");
        }

        HttpResponseMessage response;
        try
        {
            response = await TimestampDataAsync(resultData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server failed.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server timed out.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.RequestTimeout };
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
            _logger.LogInformation("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampServerTokenResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogInformation("Deserialization of timestamp token response failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult { StatusCode = HttpStatusCode.BadGateway };
        }

        var verifySignatureForData = VerifyHashWithTimestampSignatureData(disagreementHash, timestampRequest, responseObj.Data);
        if (!verifySignatureForData)
        {
            _logger.LogInformation("Timestamp signature for data verification failed for empowerment {EmpowermentId}", empowerment.Id);
            return InternalServerError("Timestamp signature for data verification failed.");
        }

        empowermentDisagreement.TimestampData = responseObj.Data;

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
                Uids = empowerment.AuthorizerUids.Select(au => new UserIdentifierData { Uid = au.Uid, UidType = au.UidType }),
                EventCode = Events.EmpowermentWasDisagreed.Code,
                Events.EmpowermentWasDisagreed.Translations
            }),
            _publishEndpoint.Publish<NotifyUids>(new
            {
                message.CorrelationId,
                message.EmpowermentId,
                Uids = empowerment.EmpoweredUids.Select(eu => new UserIdentifierData { Uid = eu.Uid, UidType = eu.UidType }),
                EventCode = Events.EmpowermentToMeWasDisagreed.Code,
                Events.EmpowermentToMeWasDisagreed.Translations
            })
        };

        await Task.WhenAll(messageTasks);
        _logger.LogInformation("{Uid} successfully declared disagreement with empowerment {EmpowermentId}.", message.Uid, message.EmpowermentId);
        return Ok();
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
                es.SupplierId == message.SupplierId &&
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
                   .ToListAsync()).Where(es => message.VolumeOfRepresentation.All(vor => es.VolumeOfRepresentation.Select(x => x.Code).Contains(vor)))
                   .Select(e => e.Id);

            empowermentStatements = empowermentStatements.Where(x => ids.Contains(x.Id));
        }

        empowermentStatements = empowermentStatements
            .OrderBy(es => es.Status)
            .ThenByDescending(es => !es.ExpiryDate.HasValue || (es.ExpiryDate.HasValue && es.ExpiryDate > DateTime.UtcNow))
            .ThenBy(es => es.ExpiryDate);

        // Execute query 
        var result = await PaginatedData<EmpowermentStatementResult>.CreateAsync(empowermentStatements, message.PageIndex, message.PageSize);

        result.Data.ToList().ForEach(x => x.CalculateStatusOn(message.StatusOn));

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
            _logger.LogInformation("Administration {AdministrationId} tried to deny non-existent empowerment ({EmpowermentId}).", message.AdministrationId, message.EmpowermentId);
            return NotFound<Guid>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        //Check if the empowerment with the given Id belongs to the logged DEAU
        if (empowermentStatement.SupplierId != message.AdministrationId)
        {
            _logger.LogInformation("Administration {AdministrationId} tried to deny another supplier's empowerment ({EmpowermentId}).", message.AdministrationId, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.AdministrationId}' can't be allowed to deny this empowerment.")
                }
            };
        }

        //Check if the empowerment is in status Unconfirmed or Active
        if (empowermentStatement.Status != EmpowermentStatementStatus.Unconfirmed && empowermentStatement.Status != EmpowermentStatementStatus.Active)
        {
            _logger.LogInformation("Administration {AdministrationId} tried to deny {EmpowermentStatus} empowerment ({EmpowermentId}).", message.AdministrationId, empowermentStatement.Status, message.EmpowermentId);
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
            Uids = empowermentStatement.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
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
            _logger.LogInformation("Administration {AdministrationId} tried to approve non-existent empowerment ({EmpowermentId}).", message.AdministrationId, message.EmpowermentId);
            return NotFound<Guid>(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        //Check if the empowerment with the given Id belongs to the logged DEAU
        if (empowermentStatement.SupplierId != message.AdministrationId)
        {
            _logger.LogInformation("Administration {AdministrationId} tried to approve another supplier's empowerment ({EmpowermentId}).", message.AdministrationId, message.EmpowermentId);
            return new ServiceResult<Guid>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Forbidden", $"'{message.AdministrationId}' can't be allowed to approve this empowerment.")
                }
            };
        }

        //Check if the empowerment is in status Unconfirmed 
        if (empowermentStatement.Status != EmpowermentStatementStatus.Unconfirmed)
        {
            _logger.LogInformation("Administration {AdministrationId} tried to approve {EmpowermentStatus} empowerment ({EmpowermentId}).", message.AdministrationId, empowermentStatement.Status, message.EmpowermentId);
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
            Uids = empowermentStatement.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
            EventCode = Events.EmpowermentCompleted.Code,
            Events.EmpowermentCompleted.Translations
        });

        // Result
        return Ok(empowermentStatement.Id);
    }

    public async Task<ServiceResult> SignEmpowermentAsync(SignEmpowerment message)
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
            return BadRequest(validationResult.Errors);
        }

        var empowerment = await _context.EmpowermentStatements
            .Include(es => es.AuthorizerUids)
            .AsSplitQuery()
            .FirstOrDefaultAsync(es => es.Id == message.EmpowermentId);

        if (empowerment == null)
        {
            _logger.LogInformation("No empowerment with id {EmpowermentId} found.", message.EmpowermentId);
            return NotFound(nameof(message.EmpowermentId), message.EmpowermentId);
        }

        var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        // Check if person signing is authorizer
        var xmlEmpowerment = XMLSerializationHelper.DeserializeEmpowermentStatementItem(empowerment.XMLRepresentation);
        if (!empowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType)
            || !xmlEmpowerment.AuthorizerUids.Any(au => au.Uid == message.Uid && au.UidType == message.UidType))
        {
            _logger.LogInformation("{Uid} forbidden to sign empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult
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
            _logger.LogInformation("{Uid} tried signing {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("Invalid status", $"'Only {EmpowermentStatementStatus.CollectingAuthorizerSignatures} empowerments can be signed.")
                }
            };
        }

        var verifySignatureServiceResult = await _verificationService.VerifySignatureAsync(empowerment.XMLRepresentation, message.DetachedSignature, message.Uid, message.UidType, message.SignatureProvider);
        if (verifySignatureServiceResult.StatusCode != HttpStatusCode.OK)
        {
            return verifySignatureServiceResult;
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
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{Uid} failed signing empowerment {EmpowermentId}.", maskedUid, message.EmpowermentId);
            return UnhandledException();
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
            _logger.LogInformation("{RequestName} validation failed. {Errors}", nameof(GetExpiringEmpowermentsRequest), validationResult);
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
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetEmpowermentsByEik), validationResult);
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

        if (!string.IsNullOrWhiteSpace(message.SupplierName))
        {
            empowermentStatements = empowermentStatements.Where(es => es.SupplierName.ToLower().Contains(message.SupplierName.ToLower()));
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
                    EmpowermentsByEikSortBy.SupplierName => empowermentStatements.OrderByDescending(es => es.SupplierName),
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
                    EmpowermentsByEikSortBy.SupplierName => empowermentStatements.OrderBy(es => es.SupplierName),
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

            //NTR Check if requester is part of legal entity's representatives
            var verificationResult = await _verificationService.VerifyRequesterInLegalEntityAsync(new CheckLegalEntityInNTRData
            {
                CorrelationId = message.CorrelationId,
                Uid = message.Eik,                                                     // Eik of the legal entity. Used to match with data in TR.
                Name = result.Data.First().Name,                                       // Name of the legal entity. Used to match with data in TR.
                IssuerName = message.IssuerName,                                       // Name of the requester. Used to match with data in TR.
                IssuerPosition = nameof(CheckLegalEntityInNTRData.IssuerPosition),     // TODO: Not used at the moment
                IssuerUid = message.IssuerUid,                                         // Uid of the requester. Used to match with data in TR.
                IssuerUidType = message.IssuerUidType,                                 // UidType of the requester. Used to match with data in TR.
            });

            //If there is a problem verifying requester or he is not part of legal entity's representation - access is forbidden
            if (verificationResult is null || verificationResult.Result is null || !verificationResult.Result.Successfull)
            {
                var bulstatResult = await _verificationService.CheckLegalEntityInBulstatAsync(new CheckLegalEntityInBulstatRequest
                {
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

    public async Task<bool> ConfirmAuthorizersInLegalEntityRepresentationAsync(Guid empowermentId, LegalEntityActualState legalEntityActualState)
    {
        if (legalEntityActualState is null)
        {
            throw new ArgumentNullException(nameof(legalEntityActualState));
        }

        if (Guid.Empty == empowermentId)
        {
            throw new ArgumentNullException(nameof(empowermentId));
        }

        var wOr = GetWayOfRepresentation(legalEntityActualState);

        var authorizerUids = await _context.AuthorizerUids
            .Where(a => a.EmpowermentStatementId == empowermentId)
            .Select(a => new UserIdentifierWithNameData
            {
                Uid = a.Uid,
                UidType = a.UidType,
                Name = a.Name,
            })
            .ToListAsync();

        var representativeRecords = legalEntityActualState?.Response?.ActualStateResponseV3?.Deed?
            .Subdeeds?.Subdeed?.FirstOrDefault()?.Records?.Record
            .Where(r => r.MainField?.MainFieldIdent == _representativeField1Id || r.MainField?.MainFieldIdent == _representativeField2Id || r.MainField?.MainFieldIdent == _representativeField3Id)
            .Select(x =>
            {
                return JsonConvert.DeserializeObject<Representative>(x?.RecordData?["representative"]?.ToString()) ?? new Representative();
            });

        if (!authorizerUids.Any() || !representativeRecords.Any())
        {
            _logger.LogInformation("Missing Authorizers or Representatives information");
            return false;
        }

        //When Jointly we have to ensure authorziers legal entity representatives fully match. Count + Uids
        if (wOr.Jointly == true && wOr.Severally == false)
        {
            if (authorizerUids.Count != representativeRecords.Count())
            {
                _logger.LogInformation("Authorizers and Representatives count mismatch");
                return false;
            }
        }

        //In every other case we have to check if all authorizers are present among Representatives
        foreach (var authorizer in authorizerUids)
        {
            if (!representativeRecords.Any(x => x?.Subject?.Indent == authorizer.Uid &&
                                                x?.Subject?.Name?.ToLower().Trim() == authorizer.Name.ToLower().Trim()))
            {
                _logger.LogInformation("Authorizer is missing from Representatives", authorizer);
                return false;
            }
        }

        _logger.LogInformation("All authorizers match with Representatives");
        return true;
    }

    public WayOfRepresentation GetWayOfRepresentation(LegalEntityActualState legalEntityActualState)
    {
        var wayOfRepresentation = legalEntityActualState?.Response?.ActualStateResponseV3?.Deed?.Subdeeds?.Subdeed?
           .FirstOrDefault()?.Records?.Record
           .FirstOrDefault(r => r.MainField?.MainFieldIdent == _wayOfRepresentationFieldId);

        var wOrD = wayOfRepresentation?.RecordData?["wayOfRepresentation"];

        var wOr = default(WayOfRepresentation);
        try
        {
            wOr = JsonConvert.DeserializeObject<WayOfRepresentation>(wOrD.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Way of representation data malformed.");
            throw;
        }

        return wOr;
    }

    public async Task<EmpowermentStatementResult> GetEmpowermentStatementByIdAsync(Guid id)
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

    public async Task<ServiceResult<TimestampingOutcome>> TimestampEmpowermentXmlAsync(Guid id)
    {
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
            _logger.LogInformation("Attempt to timestamp already timestamped Empowerment {EmpowermentId}", id);
            return Conflict<TimestampingOutcome>(nameof(EmpowermentStatement.Timestamp), id);
        }

        using var sha256 = SHA256.Create();
        var xmlHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(empowermentStatement.XMLRepresentation));
        var timestampRequest = Rfc3161TimestampRequest.CreateFromData(xmlHash, HashAlgorithmName.SHA256, requestSignerCertificates: true);
        
        // I've observed that regardless of hashSize, encoded result was always 59 bytes
        var resultData = new byte[64];
        if (!timestampRequest.TryEncode(resultData, out int OK))
        {
            _logger.LogInformation("Failed encoding timestamp request for empowerment {EmpowermentId}", id);
            return InternalServerError<TimestampingOutcome>("Failed encoding result data.");
        }

        HttpResponseMessage response;
        try
        {
            response = await TimestampDataAsync(resultData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server failed.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server timed out.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.RequestTimeout };
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
            _logger.LogInformation("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampServerTokenResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogInformation("Deserialization of timestamp token response failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        Rfc3161TimestampToken timestampToken = timestampRequest.ProcessResponse(Convert.FromBase64String(responseObj.Data), out _);
        var verifySignatureForData = VerifyHashWithTimestampSignatureData(xmlHash, timestampRequest, responseObj.Data);
        if (!verifySignatureForData)
        {
            _logger.LogInformation("Timestamp signature for data verification failed for empowerment {EmpowermentId}", id);
            return InternalServerError<TimestampingOutcome>("Timestamp signature for data verification failed.");
        }

        // Store to DB
        try
        {
            var dbEntity = await _context.EmpowermentTimestamps.AddAsync(new EmpowermentTimestamp
            {
                EmpowermentStatement = empowermentStatement,
                Data = responseObj.Data,
                DateTime = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException postgresException && postgresException.SqlState == "23505")
            {
                _logger.LogInformation("Attempt to timestamp already timestamped Empowerment {EmpowermentId}", id);
                return Conflict<TimestampingOutcome>(nameof(EmpowermentStatement.Timestamp), id);
            }
            _logger.LogInformation(ex, "DBUpdateException during timestamp saving for Empowerment {EmpowermentId}", id);
            return InternalServerError<TimestampingOutcome>("Persisting timestamp data error.");
        }

        return Ok(new TimestampingOutcome { Successful = true });
    }

    public async Task<ServiceResult<TimestampingOutcome>> TimestampEmpowermentWithdrawalAsync(Guid empowermentId, Guid withdrawalId)
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
            _logger.LogInformation("Attempt to timestamp already timestamped Empowerment {EmpowermentId}  Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return Conflict<TimestampingOutcome>(nameof(EmpowermentWithdrawal.TimestampData), empowermentId);
        }

        using var sha256 = SHA256.Create();
        var withdrawalHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(empowermentWithdrawal.ToString()));
        var timestampRequest = Rfc3161TimestampRequest.CreateFromData(withdrawalHash, HashAlgorithmName.SHA256, requestSignerCertificates: true);

        // I've observed that regardless of hashSize, encoded result was always 59 bytes
        var resultData = new byte[64];
        if (!timestampRequest.TryEncode(resultData, out int OK))
        {
            _logger.LogInformation("Failed encoding result data for empowerment {EmpowermentId}  Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return InternalServerError<TimestampingOutcome>("Failed encoding result data.");
        }

        HttpResponseMessage response;
        try
        {
            response = await TimestampDataAsync(resultData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server failed.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server timed out.");
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.RequestTimeout };
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
            _logger.LogInformation("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampServerTokenResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogInformation("Deserialization of timestamp token response failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<TimestampingOutcome> { StatusCode = HttpStatusCode.BadGateway };
        }

        var verifySignatureForData = VerifyHashWithTimestampSignatureData(withdrawalHash, timestampRequest, responseObj.Data);
        if (!verifySignatureForData)
        {
            _logger.LogInformation("Timestamp signature for data verification failed for empowerment {EmpowermentId} Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return InternalServerError<TimestampingOutcome>("Timestamp signature for data verification failed.");
        }

        // Store to DB
        try
        {
            if (_context.EmpowermentWithdrawals.Any(ew => ew.Id == withdrawalId && ew.EmpowermentStatementId == empowermentId && ew.TimestampData.Length > 0))
            {
                _logger.LogInformation("Attempt to timestamp already timestamped Empowerment {EmpowermentId} Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
                return Conflict<TimestampingOutcome>(nameof(EmpowermentWithdrawal.TimestampData), withdrawalId);
            }

            empowermentWithdrawal.TimestampData = responseObj.Data;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "DBUpdateException during timestamp saving for Empowerment {EmpowermentId} Withdrawal {WithdrawalId}", empowermentId, withdrawalId);
            return InternalServerError<TimestampingOutcome>("Persisting withdrawal timestamp data error.");
        }

        return Ok(new TimestampingOutcome { Successful = true });
    }

    private async Task<HttpResponseMessage> TimestampDataAsync(byte[] resultData)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _timestampServerOptions.RequestTokenUrl);
        request.Headers.TryAddWithoutValidation(HeaderNames.ContentType, System.Net.Mime.MediaTypeNames.Application.Json);
        request.Content = System.Net.Http.Json.JsonContent.Create(new { data = Convert.ToBase64String(resultData), encoding = "BASE64" });

        var response = await _timestampServerHttpClient.SendAsync(request);
        return response;
    }

    /// <param name="sha256Hash">SHA256 hash that was timestamped</param>
    /// <param name="timestampRequest"></param>
    /// <param name="timestampingData">Base64 encoded data. Returned from timestamp server after successful timestamping.</param>
    /// <returns></returns>
    private static bool VerifyHashWithTimestampSignatureData(byte[] sha256Hash, Rfc3161TimestampRequest timestampRequest, string timestampingData)
    {
        Rfc3161TimestampToken timestampToken = timestampRequest.ProcessResponse(Convert.FromBase64String(timestampingData), out _);
        return timestampToken.VerifySignatureForData(sha256Hash, out X509Certificate2 _);
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
        public IEnumerable<UserIdentifierWithName> AuthorizerUids { get; set; }
        public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }


}
