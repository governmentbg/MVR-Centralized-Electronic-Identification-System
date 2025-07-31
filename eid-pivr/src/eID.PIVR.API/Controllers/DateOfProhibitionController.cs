using eID.PIVR.API.Authorization;
using eID.PIVR.API.Requests;
using eID.PIVR.Contracts;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PIVR.API.Controllers;

[RoleAuthorization(UserRoles.Operator, UserRoles.AdministratorElectronicIdentity, UserRoles.RuMvrAdministrator)]
public class DateOfProhibitionController : BaseV1Controller
{
    public DateOfProhibitionController(IConfiguration configuration, ILogger<DateOfProhibitionController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Query date of prohibition by personal id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="personIdQuery"></param>
    /// <returns>Date when the person got under prohibition; null if the person is still capable</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateOfProhibitionResult))]
    public async Task<IActionResult> GetDateOfProhibitionAsync(
        [FromServices] IRequestClient<GetDateOfProhibition> client,
        [FromQuery] GetByPersonalIdQuery personIdQuery,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_DATE_OF_PROHIBITION;
        var eventPayload = BeginAuditLog(logEventCode, personIdQuery,
            (AuditLoggingKeys.TargetUid, personIdQuery.PersonalId),
            (AuditLoggingKeys.TargetUidType, personIdQuery.UidType.ToString()),
            (AuditLogHelper.Source, AuditLogHelper.DatabaseNaif));
        if (!personIdQuery.IsValid())
        {
            return BadRequestWithAuditLog(personIdQuery, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<DateOfProhibitionResult>>(
                new
                {
                    CorrelationId = RequestId,
                    personIdQuery.PersonalId,
                    personIdQuery.UidType,
                }, cancellationToken));


        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
