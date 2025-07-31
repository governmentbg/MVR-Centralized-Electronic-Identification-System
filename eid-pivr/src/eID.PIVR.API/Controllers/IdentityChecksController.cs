using eID.PIVR.API.Requests;
using eID.PIVR.Contracts;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PIVR.API.Controllers;

public class IdentityChecksController : BaseV1Controller
{
    private readonly IApiUsageTrackerService _usageTracker;

    public IdentityChecksController(IConfiguration configuration, ILogger<IdentityChecksController> logger, AuditLogger auditLogger, IApiUsageTrackerService usageTracker) : base(configuration, logger, auditLogger)
    {
        _usageTracker = usageTracker ?? throw new ArgumentNullException(nameof(usageTracker));
    }

    /// <summary>
    /// Get Id changes.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("idchanges", Name = nameof(GetIdChangesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IdChangeResult>))]
    public async Task<IActionResult> GetIdChangesAsync(
        [FromServices] IRequestClient<GetIdChanges> client,
        [FromQuery] GetIdChangesRequest request,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Национален автоматизиран информационен фонд за българските лични документи");
        var logEventCode = LogEventCode.GET_ID_CHANGES;
        SortedDictionary<string, object> eventPayload;
        if (request.PersonalId != null || request.UidType.HasValue)
        {
            eventPayload = BeginAuditLog(logEventCode, request,
                (AuditLoggingKeys.TargetUid, request.PersonalId),
                (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
                (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        }
        else
        {
            eventPayload = BeginAuditLog(logEventCode, request, (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        }
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<IdChangeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PersonalId,
                    request.UidType,
                    request.CreatedOnFrom,
                    request.CreatedOnTo,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get statut changes
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("statut", Name = nameof(GetStatutChangesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StatutChangeResult>))]
    public async Task<IActionResult> GetStatutChangesAsync(
        [FromServices] IRequestClient<GetStatutChanges> client,
        [FromQuery] GetStatutChangesRequest request,
        CancellationToken cancellationToken)
    {
        await _usageTracker.TrackUsageAsync("Национален автоматизиран информационен фонд за българските лични документи");
        var logEventCode = LogEventCode.GET_STATUT_CHANGES;
        SortedDictionary<string, object> eventPayload;
        if (request.PersonalId != null || request.UidType.HasValue)
        {
            eventPayload = BeginAuditLog(logEventCode, request,
                (AuditLoggingKeys.TargetUid, request.PersonalId),
                (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
                (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        }
        else
        {
            eventPayload = BeginAuditLog(logEventCode, request, (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        }
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<StatutChangeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PersonalId,
                    request.UidType,
                    request.CreatedOnFrom,
                    request.CreatedOnTo,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
