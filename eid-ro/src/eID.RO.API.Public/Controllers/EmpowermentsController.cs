using System.Net.Mime;
using System.Text;
using eID.PJS.AuditLogging;
using eID.RO.API.Public.Exports;
using eID.RO.API.Public.Extensions;
using eID.RO.API.Public.Options;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.RO.API.Public.Controllers;

//[ApiExplorerSettings(IgnoreApi = true)] // Uncomment when create SDK
public class EmpowermentsController : BaseV1Controller
{
    private ApiOptions _apiOptions;
    private HttpClient _httpClient;

    /// <summary>
    /// Crate an instance of <see cref="EmpowermentsController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    /// <param name="apiOptions"></param>    
    public EmpowermentsController(
        IConfiguration configuration,
        ILogger<EmpowermentsController> logger,
        AuditLogger auditLogger,
        IOptions<ApiOptions> apiOptions,
        IHttpClientFactory httpClientFactory) : base(configuration, logger, auditLogger)
    {
        _apiOptions = (apiOptions ?? throw new ArgumentNullException(nameof(apiOptions))).Value;
        _httpClient = httpClientFactory.CreateClient("Signing");
    }

    /// <summary>
    /// Create empowerment statements
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of ids of all created statements (one or more)</returns>
    [HttpPost(Name = nameof(CreateStatementsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Guid>))]
    public async Task<IActionResult> CreateStatementsAsync(
        [FromServices] IRequestClient<AddEmpowermentStatement> client,
        [FromBody] AddEmpowermentStatementsRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var logEventCode = LogEventCode.CREATE_EMPOWERMENT;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
            { nameof(request.ProviderName), request.ProviderName },
            { nameof(request.ServiceName), request.ServiceName }
        };

        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId: targetUserId, payload: eventPayload); // Audit log for the creator

        request.Uid = request.OnBehalfOf == OnBehalfOf.LegalEntity ? request.Uid : loggedUserUid;
        request.UidType = request.OnBehalfOf == OnBehalfOf.LegalEntity ? IdentifierType.NotSpecified : loggedUserUidType;
        request.Name = request.OnBehalfOf == OnBehalfOf.LegalEntity ? request.Name : loggedUserName;
        request.AuthorizerUids.Insert(0, new AuthorizerIdentifierData { Uid = loggedUserUid, UidType = loggedUserUidType, Name = loggedUserName, IsIssuer = true });
        foreach (var item in request.AuthorizerUids)
        {
            item.Name = System.Text.RegularExpressions.Regex.Replace(item.Name, @"\s+", " ").Trim();
        }
        foreach (var item in request.EmpoweredUids)
        {
            item.Name = System.Text.RegularExpressions.Regex.Replace(item.Name, @"\s+", " ").Trim();
        }
        foreach (UserIdentifierData empowered in request.EmpoweredUids)
        {
            var currentPayload = new SortedDictionary<string, object>(eventPayload)
            {
                [AuditLoggingKeys.TargetUid] = empowered.Uid,
                [AuditLoggingKeys.TargetUidType] = empowered.UidType.ToString(),
                [AuditLoggingKeys.TargetName] = empowered.Name
            };
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId: targetUserId, payload: currentPayload);
        }

        var validator = new AddEmpowermentStatementRequestValidator(_apiOptions.AllowSelfEmpowerment);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var err = validationResult.GetValidationErrorList();
            eventPayload.Add("Reason", string.Join(",", err));
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.BadRequest);
            foreach (UserIdentifierData empowered in request.EmpoweredUids)
            {
                var currentPayload = new SortedDictionary<string, object>(eventPayload)
                {
                    [AuditLoggingKeys.TargetUid] = empowered.Uid,
                    [AuditLoggingKeys.TargetUidType] = empowered.UidType.ToString(),
                    [AuditLoggingKeys.TargetName] = empowered.Name
                };
                AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, targetUserId: targetUserId, payload: currentPayload);
            }
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, targetUserId: targetUserId, payload: eventPayload); // Audit log for the creator

            return BadRequest(validationResult);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<Guid>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.OnBehalfOf,
                    request.Uid,
                    request.UidType,
                    request.Name,
                    request.AuthorizerUids,
                    request.EmpoweredUids,
                    request.TypeOfEmpowerment,
                    request.ProviderId,
                    request.ProviderName,
                    request.ServiceId,
                    request.ServiceName,
                    request.IssuerPosition,
                    request.VolumeOfRepresentation,
                    request.StartDate,
                    request.ExpiryDate,
                    CreatedBy = targetUserId,
                    _apiOptions.AllowSelfEmpowerment
                }, cancellationToken));

        return Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }

            foreach (UserIdentifierData empowered in request.EmpoweredUids)
            {
                var currentPayload = new SortedDictionary<string, object>(eventPayload)
                {
                    [AuditLoggingKeys.TargetUid] = empowered.Uid,
                    [AuditLoggingKeys.TargetUidType] = empowered.UidType.ToString(),
                    [AuditLoggingKeys.TargetName] = empowered.Name
                };
                AddAuditLog(logEventCode, suffix: suffix, targetUserId: targetUserId, payload: currentPayload);
            }
            AddAuditLog(logEventCode, suffix: suffix, targetUserId: targetUserId, payload: eventPayload); // Audit log for the creator
        });
    }

    /// <summary>
    /// Query Database only for Empowerment Statements which are created "To Me" by provided filter
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Returns a list of all "To Me" Empowerments Statements which match the filter criteria</returns>
    [HttpGet("to", Name = nameof(GetEmpowermentsToMeByFilterAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementResult>))]
    public async Task<IActionResult> GetEmpowermentsToMeByFilterAsync(
        [FromServices] IRequestClient<GetEmpowermentsToMeByFilter> client,
        CancellationToken cancellationToken,
        [FromQuery] GetEmpowermentsToMeByFilterRequest request)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENTS_TO_ME_BY_FILTER;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId: targetUserId, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Number,
                    request.Status,
                    request.Authorizer,
                    request.ProviderName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.SortBy,
                    request.SortDirection,
                    Uid = loggedUserUid,
                    UidType = loggedUserUidType,
                    request.OnBehalfOf,
                    request.Eik,
                    request.PageIndex,
                    request.PageSize
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload, targetUserId);
    }

    /// <summary>
    /// Handles detached signature for the empowerment from the user.
    /// Accepting the signature after validations.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="empowermentId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{empowermentId}/sign", Name = nameof(SignEmpowermentAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignEmpowermentAsync(
        [FromServices] IRequestClient<SignEmpowerment> client,
        [FromRoute] Guid empowermentId,
        [FromBody] SignEmpowermentPayload payload,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SIGN_EMPOWERMENT;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, payload },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
            { "EmpowermentId", empowermentId }
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId, payload: eventPayload);
        payload ??= new SignEmpowermentPayload();
        if (!payload.IsValid())
        {
            return BadRequestWithAuditLog(payload, logEventCode, eventPayload, targetUserId);
        }
        var request = new SignEmpowermentRequest
        {
            Name = loggedUserName,
            Uid = loggedUserUid,
            UidType = loggedUserUidType,
            EmpowermentId = empowermentId
        };
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<string>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Name,
                    request.Uid,
                    request.UidType,
                    request.EmpowermentId,
                    payload.DetachedSignature,
                    payload.SignatureProvider,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload, targetUserId);
    }

    /// <summary>
    /// THIS IS AN ENDPOINT INTENDED FOR DEV PURPOSES ONLY!!!
    /// It enables us to fake signing before actual functionality is in place.
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="empowermentId"></param>
    /// <param name="name"></param>
    /// <param name="uid">The "uid" that signed the empowerment</param>
    /// <param name="uidType"></param>
    [HttpGet("{empowermentId}/sign")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TestSignAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromRoute] Guid empowermentId,
        [FromQuery] string name,
        [FromQuery] string uid,
        [FromQuery] IdentifierType uidType)
    {
        await publishEndpoint.Publish<Contracts.Events.EmpowermentSigned>(new
        {
            CorrelationId = RequestId,
            EmpowermentId = empowermentId,
            SignerName = name,
            SignerUid = uid,
            SignerUidType = uidType
        });
        return Ok();
    }

    /// <summary>
    /// Query Database only for Empowerment Statements which are created "From Me" by provided filter
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Returns a list of all "From Me" Empowerments Statements which match the filter criteria</returns>
    [HttpPost("from", Name = nameof(GetEmpowermentsFromMeByFilterAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementFromMeResult>))]
    public async Task<IActionResult> GetEmpowermentsFromMeByFilterAsync(
        [FromServices] IRequestClient<GetEmpowermentsFromMeByFilter> client,
        GetEmpowermentsFromMeByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENTS_FROM_ME_BY_FILTER;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementFromMeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Number,
                    request.Status,
                    request.Authorizer,
                    request.ProviderName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.SortBy,
                    request.SortDirection,
                    request.EmpoweredUids,
                    request.OnBehalfOf,
                    Uid = loggedUserUid,
                    UidType = loggedUserUidType,
                    request.Eik,
                    request.PageIndex,
                    request.PageSize
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload, targetUserId);
    }

    /// <summary>
    /// During withdrawal process the citizen can select reason for withdrawal from a predefined list
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of predefined reasons</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmpowermentWithdrawalReasonResult>))]
    [HttpGet("withdraw/reasons", Name = nameof(GetWithdrawReasonsAsync))]
    public async Task<IActionResult> GetWithdrawReasonsAsync(
        [FromServices] IRequestClient<GetEmpowermentWithdrawReasons> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<EmpowermentWithdrawalReasonResult>>>(
                new
                {
                    CorrelationId = RequestId,
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Initiate empowerment withdraw process or any from AuthorizerUids can confirm this withdraw
    /// </summary>
    /// <param name="client"></param>
    /// <param name="empowermentId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Outfcome of withdraw operation in terms of http status code.</returns>
    [HttpPost("{empowermentId}/withdraw", Name = nameof(WithdrawEmpowermentAsync))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> WithdrawEmpowermentAsync(
        [FromServices] IRequestClient<WithdrawEmpowerment> client,
        [FromRoute] Guid empowermentId,
        [FromBody] WithdrawEmpowermentRequestPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new WithdrawEmpowermentRequest
        {
            Uid = GetUid(),
            UidType = GetUidType(),
            EmpowermentId = empowermentId,
            Reason = payload.Reason,
            Name = GetUserFullName()
        };
        var logEventCode = LogEventCode.WITHDRAW_EMPOWERMENT;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId, payload: eventPayload);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<string>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid,
                    request.UidType,
                    request.EmpowermentId,
                    request.Reason,
                    request.Name
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult.AsPlainServiceResult(), logEventCode, eventPayload, targetUserId);
    }

    /// <summary>
    /// During disagreement process the citizen can select reason for disagreement from a predefined list
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of predefined reasons</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmpowermentDisagreementReasonResult>))]
    [HttpGet("disagreement/reasons", Name = nameof(GetDisagreementReasonsAsync))]
    public async Task<IActionResult> GetDisagreementReasonsAsync(
        [FromServices] IRequestClient<GetEmpowermentDisagreementReasons> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<EmpowermentDisagreementReasonResult>>>(
                new
                {
                    CorrelationId = RequestId,
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Declare disagreement with empowerment. Can be performed by any empowered people
    /// </summary>
    /// <param name="client"></param>
    /// <param name="empowermentId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Outcome of disagreement operation.</returns>
    [HttpPost("{empowermentId}/disagreement", Name = nameof(DisagreeEmpowermentAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DisagreeEmpowermentAsync(
        [FromServices] IRequestClient<DisagreeEmpowerment> client,
        [FromRoute] Guid empowermentId,
        [FromBody] DisagreeEmpowermentRequestPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new DisagreeEmpowermentRequest
        {
            Uid = GetUid(),
            UidType = GetUidType(),
            EmpowermentId = empowermentId,
            Reason = payload.Reason,
            Name = GetUserFullName()
        };
        var logEventCode = LogEventCode.DISAGREE_EMPOWERMENT;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, targetUserId, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<string>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid,
                    request.UidType,
                    request.EmpowermentId,
                    request.Reason,
                    request.Name,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult.AsPlainServiceResult(), logEventCode, eventPayload, targetUserId);
    }

    /// <summary>
    /// Query Database only for Empowerment Statements based on EIK
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Returns a list of all Empowerments Statements, filtered for given Eik and/or other filters</returns>
    [HttpPost("eik", Name = nameof(GetEmpowermentsByEikAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmpowermentsByEikAsync(
        [FromServices] IRequestClient<GetEmpowermentsByEik> client,
        GetEmpowermentsByEikFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENTS_BY_EIK;
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
            { nameof(request.Eik), request.Eik },
        };
        var targetUserId = GetUserId();
        HttpContext.Items["TargetUserId"] = targetUserId;
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload, targetUserId: targetUserId);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, targetUserId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Eik,
                    IssuerUid = loggedUserUid,
                    IssuerUidType = loggedUserUidType,
                    IssuerName = loggedUserName,
                    request.Status,
                    request.ProviderName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.AuthorizerUids,
                    request.PageIndex,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload, targetUserId);
    }

    [HttpGet("{empowermentId}/document/to", Name = nameof(GetEmpowermentDocumentToMeAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpowermentDocumentToMeAsync(
        [FromServices] IRequestClient<GetEmpowermentById> client,
        [FromRoute] Guid empowermentId,
        [FromQuery] LanguageType language,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENT_DOCUMENT_TO_ME;

        return await GetEmpowermentDocumentIntAsync(client, empowermentId, logEventCode, true, language, cancellationToken);
    }

    [HttpGet("{empowermentId}/document/from", Name = nameof(GetEmpowermentDocumentFromMeAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpowermentDocumentFromMeAsync(
        [FromServices] IRequestClient<GetEmpowermentById> client,
        [FromRoute] Guid empowermentId,
        [FromQuery] LanguageType language,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENT_DOCUMENT_FROM_ME;

        return await GetEmpowermentDocumentIntAsync(client, empowermentId, logEventCode, false, language, cancellationToken);
    }

    private async Task<IActionResult> GetEmpowermentDocumentIntAsync(
        IRequestClient<GetEmpowermentById> client,
        Guid empowermentId,
        LogEventCode logEventCode,
        bool isToMe,
        LanguageType language,
        CancellationToken cancellationToken)
    {
        var loggedUserUid = GetUid();
        var loggedUserUidType = GetUidType();
        var loggedUserName = GetUserFullName();
        var eventPayload = new SortedDictionary<string, object>
        {
            { "EmpowermentId", empowermentId },
            { AuditLoggingKeys.RequesterUid, loggedUserUid },
            { AuditLoggingKeys.RequesterUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.RequesterName, loggedUserName },
            { AuditLoggingKeys.TargetUid, loggedUserUid },
            { AuditLoggingKeys.TargetUidType, loggedUserUidType.ToString() },
            { AuditLoggingKeys.TargetName, loggedUserName },
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);

        if (empowermentId == Guid.Empty)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("EmpowermentId", "EmpowermentId is required");
            eventPayload.Add("Reason", "EmpowermentId is required");
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<EmpowermentStatementWithSignaturesResult>>(
                new
                {
                    CorrelationId = RequestId,
                    EmpowermentId = empowermentId
                }, cancellationToken));

        if (serviceResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return Result(serviceResult, (errorMessage, suffix, statusCode) =>
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    eventPayload.Add("Reason", errorMessage);
                }
                if (statusCode is not null)
                {
                    eventPayload.Add("ResponseStatusCode", statusCode);
                }
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            });
        }
        var empowerment = serviceResult.Result;
        empowerment?.CalculateStatusOn(DateTime.Now);

        var isUserValid = false;
        var isStatusValid = false;
        if (empowerment != null)
        {
            if (isToMe) // To Me
            {
                isUserValid = empowerment.EmpoweredUids.Any(au => au.Uid == loggedUserUid && au.UidType == loggedUserUidType);

                var invalidStates = new HashSet<EmpowermentStatementStatus>
                {
                    EmpowermentStatementStatus.Created,
                    EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                    EmpowermentStatementStatus.Denied,
                };

                isStatusValid = !invalidStates.Contains(empowerment.Status);
            }
            else // For me
            {
                isUserValid = empowerment.AuthorizerUids.Any(au => au.Uid == loggedUserUid && au.UidType == loggedUserUidType);
                isStatusValid = true;
            }
        }

        if (empowerment is null || !isUserValid || !isStatusValid)
        {
            Logger.LogError("Empowerment {EmpowermentId} was null.", empowermentId);
            eventPayload.Add("Reason", $"Empowerment {empowermentId} is null.");
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.NotFound);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return NotFound();
        }

        var pdfBytes = PdfBulder.CreateEmpowermentDocument(language, empowerment);

        foreach (var currentUser in empowerment.AuthorizerUids.Union(empowerment.EmpoweredUids).ToHashSet(new UidResultComparer())) // Distinct list of all authorizers and empowered people data
        {
            var currentPayload = new SortedDictionary<string, object>(eventPayload)
            {
                [AuditLoggingKeys.TargetUid] = currentUser.Uid,
                [AuditLoggingKeys.TargetUidType] = currentUser.UidType.ToString(),
                [AuditLoggingKeys.TargetName] = currentUser.Name,
                [nameof(empowerment.ProviderName)] = empowerment?.ProviderName ?? "Unable to obtain ProviderName"
            };
            AddAuditLog(logEventCode, targetUserId: empowerment?.CreatedBy, suffix: LogEventLifecycle.SUCCESS, payload: currentPayload);
        }

        var requestBody = new
        {
            Contents = new[]
            {
                new
                {
                    MediaType = MediaTypeNames.Application.Pdf,
                    Data = Convert.ToBase64String(pdfBytes),
                    FileName = $"{empowermentId}.pdf",
                    SignatureType = "PADES_BASELINE_B"
                    }
                }
        };

        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.PostAsync(
            "/api/v1/borica/sign",
            new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, MediaTypeNames.Application.Json),
            cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        HttpContext.Response.RegisterForDispose(response);

        var result = Newtonsoft.Json.Linq.JObject.Parse(body);
        if (result is null || result.Value<string>("responseCode") != "COMPLETED")
        {
            Logger.LogDebug("Borica sign raw response: {RawResponse}", body);
            Logger.LogWarning("Borica signing request wasn't completed.");
            return StatusCode((int)System.Net.HttpStatusCode.BadGateway, "Malformed response.");
        }
        var signature = result["data"]["signatures"][0].Value<string>("signature");

        if (string.IsNullOrWhiteSpace(signature))
        {
            Logger.LogError("Signature is missing when tried to get empowerment {EmpowermentId} document.", empowermentId);
            return StatusCode((int)System.Net.HttpStatusCode.BadGateway, "Missing signature.");
        }

        byte[] byteArray = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(signature))
        {
            byteArray = Convert.FromBase64String(signature);
        }

        return new FileStreamResult(new MemoryStream(byteArray), MediaTypeNames.Application.Pdf)
        {
            FileDownloadName = $"{empowermentId}"
        };
    }
}
