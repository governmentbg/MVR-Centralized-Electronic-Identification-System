using eID.PJS.AuditLogging;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eID.RO.API.Public.Controllers;

[ClaimsCheck(Claims.CitizenIdentifier, Claims.CitizenIdentifierType, Claims.SystemId, Claims.SystemName)]
[Authorize(Policy = "Employees")]
public class DeauController : BaseV1Controller
{
    public DeauController(IConfiguration configuration, ILogger<DeauController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// This endpoint will validate Deau and search for a Empowerments based on filter.
    /// It may return either a 200 OK or 202 Accepted response.
    /// 
    /// - 202 Accepted: Indicates that validation checks for legal representation are still in progress. 
    ///   The response will contain an empty list. You should retry the request after a short interval.
    /// 
    /// - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.
    /// 
    /// Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately. 
    /// If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff) 
    /// until a 200 OK is returned with the final data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("empowerments", Name = nameof(GetEmpowermentsByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementResult>))]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(IPaginatedData<EmpowermentStatementResult>))]
    public async Task<IActionResult> GetEmpowermentsByDeauAsync(
        [FromServices] IRequestClient<GetEmpowermentsByDeau> client,
        [FromBody] GetEmpowermentsByDeauRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENTS_BY_DEAU;
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() },
            { AuditLoggingKeys.RequesterName, GetUserFullName() },
            { "ProviderId", GetSystemId() ?? "Unable to obtain ProviderId" }
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.OnBehalfOf,
                    request.AuthorizerUid,
                    request.AuthorizerUidType,
                    request.EmpoweredUid,
                    request.EmpoweredUidType,
                    ProviderId = GetSystemId(),
                    RequesterUid = GetUid(),
                    request.ServiceId,
                    request.VolumeOfRepresentation,
                    request.StatusOn,
                    request.PageSize,
                    request.PageIndex,
                    request.SortDirection,
                    request.SortBy
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
            if (serviceResult.Result?.Data.Any() == true)
            {
                foreach (var eID in serviceResult.Result.Data.Select(r => r.CreatedBy).Distinct())
                {
                    var currentEmpowerment = serviceResult.Result.Data.FirstOrDefault(r => r.CreatedBy == eID);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        [AuditLoggingKeys.TargetUid] = request.EmpoweredUid,
                        [AuditLoggingKeys.TargetUidType] = request.EmpoweredUidType.ToString(),
                        [nameof(currentEmpowerment.ProviderName)] = currentEmpowerment?.ProviderName ?? "Unable to obtain ProviderName"
                    };
                    AddAuditLog(logEventCode, targetUserId: eID, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });
    }

    /// <summary>
    /// Deny both Active and Unconfirmed empowerments
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("deny-empowerment", Name = nameof(DenyEmpowermentByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DenyEmpowermentByDeauAsync(
        [FromServices] IRequestClient<DenyEmpowermentByDeau> client,
        [FromBody] DenyEmpowermentByDeauRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.DENY_EMPOWERMENT_BY_DEAU;
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() },
            { AuditLoggingKeys.RequesterName, GetUserFullName() },
            { nameof(request.EmpowermentId), request.EmpowermentId },
            { nameof(DenyEmpowermentByDeau.ProviderId), GetSystemId() ?? "Unable to obtain ProviderId" }
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetSystemId(),
                    request.EmpowermentId,
                    request.DenialReasonComment
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Approve Unconfirmed empowerment
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("approve-empowerment", Name = nameof(ApproveEmpowermentByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveEmpowermentByDeauAsync(
        [FromServices] IRequestClient<ApproveEmpowermentByDeau> client,
        [FromBody] ApproveEmpowermentByDeauRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.APPROVE_EMPOWERMENT_BY_DEAU;
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() },
            { AuditLoggingKeys.RequesterName, GetUserFullName() },
            { nameof(request.EmpowermentId), request.EmpowermentId },
            { nameof(ApproveEmpowermentByDeau.ProviderId), GetSystemId() ?? "Unable to obtain ProviderId" }
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetSystemId(),
                    request.EmpowermentId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
