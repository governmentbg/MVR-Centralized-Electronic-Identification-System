using eID.PJS.AuditLogging;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.RO.API.Public.Controllers;

//[ApiExplorerSettings(IgnoreApi = true)] // Uncomment when create SDK
public class EmpowermentsController : BaseV1Controller
{
    /// <summary>
    /// Crate an instance of <see cref="EmpowermentsController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public EmpowermentsController(IConfiguration configuration, ILogger<EmpowermentsController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
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

        var userId = GetUserId();
        var uid = GetUid();//Check identifier, when real data is supplied
        var uidType = GetUidType();
        var fullName = GetUserFullName();//Check identifier, when real data is supplied

        request.Uid = request.OnBehalfOf == OnBehalfOf.LegalEntity ? request.Uid : uid;
        request.UidType = request.OnBehalfOf == OnBehalfOf.LegalEntity ? IdentifierType.NotSpecified : uidType;
        request.Name = request.OnBehalfOf == OnBehalfOf.LegalEntity ? request.Name : fullName;
        request.AuthorizerUids.Insert(0, new UserIdentifierWithNameData { Uid = uid, UidType = uidType, Name = fullName, IsIssuer = true });
        foreach (var item in request.AuthorizerUids)
        {
            item.Name = System.Text.RegularExpressions.Regex.Replace(item.Name, @"\s+", " ").Trim();
        }
        if (!request.IsValid())
        {
            return BadRequest(request);
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
                    request.SupplierId,
                    request.SupplierName,
                    request.ServiceId,
                    request.ServiceName,
                    request.IssuerPosition,
                    request.VolumeOfRepresentation,
                    request.StartDate,
                    request.ExpiryDate,
                    CreatedBy = userId,
                }, cancellationToken));

        AddAuditLog(LogEventCode.CreateEmpowerment, userId);

        return Result(serviceResult);
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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var egn = GetUid();//Check identifier, when real data is supplied

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Status,
                    request.Authorizer,
                    request.SupplierName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.SortBy,
                    request.SortDirection,
                    Uid = egn,
                    UidType = GetUidType(),
                    request.PageIndex,
                    request.PageSize
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetEmpowermentsToMeByFilter, GetUserId());

        return Result(serviceResult);
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
        payload ??= new SignEmpowermentPayload();
        if (!payload.IsValid())
        {
            return BadRequest(payload);
        }
        var request = new SignEmpowermentRequest
        {
            Name = GetUserFullName(),
            Uid = GetUid(),
            UidType = GetUidType(),
            EmpowermentId = empowermentId
        };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
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

        AddAuditLog(LogEventCode.SignEmpowerment, GetUserId(), payload: new SortedDictionary<string, object>
            {
                { "EmpowermentId", request.EmpowermentId },
                { "RequestUid", request.Uid }
            });
        return Result(serviceResult);
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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var egn = GetUid();//Check identifier, when real data is supplied
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementFromMeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Status,
                    request.Authorizer,
                    request.SupplierName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.SortBy,
                    request.SortDirection,
                    request.EmpoweredUids,
                    request.OnBehalfOf,
                    Uid = egn,
                    UidType = GetUidType(),
                    request.PageIndex,
                    request.PageSize
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetEmpowermentsFromMeByFilter, GetUserId());

        return Result(serviceResult);
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

        AddAuditLog(LogEventCode.GetEmpowermentWithdrawReasons);

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

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid,
                    request.UidType,
                    request.EmpowermentId,
                    request.Reason,
                    request.Name
                }, cancellationToken));

        AddAuditLog(LogEventCode.WithdrawEmpowerment, GetUserId(), payload: new SortedDictionary<string, object>
            {
                { "EmpowermentId", request.EmpowermentId },
                { "RequestUid", request.Uid }
            });
        return Result(serviceResult);
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

        AddAuditLog(LogEventCode.GetEmpowermentDisagreementReasons);

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
            Reason = payload.Reason
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid,
                    request.UidType,
                    request.EmpowermentId,
                    request.Reason
                }, cancellationToken));

        AddAuditLog(LogEventCode.DisagreeEmpowerment, GetUserId(), payload: new SortedDictionary<string, object>
            {
                { "EmpowermentId", request.EmpowermentId },
                { "RequestUid", request.Uid }
            });
        return Result(serviceResult);
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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Eik,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.Status,
                    request.SupplierName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.AuthorizerUids,
                    request.PageIndex,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetEmpowermentsByEik, GetUserId());

        return Result(serviceResult);
    }
}
