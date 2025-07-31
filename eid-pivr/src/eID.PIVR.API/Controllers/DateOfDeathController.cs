using eID.PIVR.API.Authorization;
using eID.PIVR.API.Requests;
using eID.PIVR.Contracts;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PIVR.API.Controllers;

public class DateOfDeathController : BaseV1Controller
{
    public DateOfDeathController(IConfiguration configuration, ILogger<DateOfDeathController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Query date of death by personal id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="personIdQuery"></param>
    /// <returns>Date when the person deceased; null if the person is still alive</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateOfDeathResult))]
    [RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
    public async Task<IActionResult> GetDateOfDeathAsync(
        [FromServices] IRequestClient<GetDateOfDeath> client,
        [FromQuery] GetByPersonalIdQuery personIdQuery,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_DATE_OF_DEATH;
        var eventPayload = BeginAuditLog(logEventCode, personIdQuery,
            (AuditLoggingKeys.TargetUid, personIdQuery.PersonalId),
            (AuditLoggingKeys.TargetUidType, personIdQuery.UidType.ToString()),
            (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        if (!personIdQuery.IsValid())
        {
            return BadRequestWithAuditLog(personIdQuery, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<DateOfDeathResult>>(
                new
                {
                    CorrelationId = RequestId,
                    personIdQuery.PersonalId,
                    personIdQuery.UidType,
                }, cancellationToken));


        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get all deceased persons by period
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("deceasedByPeriod")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DeceasedByPeriodResult>))]
    public async Task<IActionResult> GetDeceasedByPeriodAsync(
        [FromServices] IRequestClient<GetDeceasedByPeriod> client,
        [FromQuery] GetDeceasedByPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_DECEASED_BY_PERIOD;
        var eventPayload = BeginAuditLog(logEventCode, request, (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<DeceasedByPeriodResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.From,
                    request.To
                }, cancellationToken));

        return ResultWithAuditLogFromDeceasedByPeriodResult(logEventCode, eventPayload, serviceResult, serviceResult?.Result);
    }

    private IActionResult ResultWithAuditLogFromDeceasedByPeriodResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IEnumerable<DeceasedByPeriodResult>> serviceResult, IEnumerable<DeceasedByPeriodResult>? data)
        => Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }
            if (data?.Any() == true)
            {
                foreach (var uid in data.Select(r => r.PersonalId).Distinct())
                {
                    var deceased = data.OrderByDescending(r => r.Date).First(r => r.PersonalId == uid); // Taking the latest result
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        [AuditLoggingKeys.TargetUid] = deceased.PersonalId,
                        [AuditLoggingKeys.TargetUidType] = deceased.UidType.ToString()
                    };
                    AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });
}
