using eID.MIS.API.Authorization;
using eID.MIS.API.Requests;
using eID.MIS.Contracts.Requests;
using eID.MIS.Contracts.Results;
using eID.MIS.Contracts.SEV.Commands;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.MIS.API.Controllers;

/// <summary>
/// Support system of open data
/// </summary>
//[ApiExplorerSettings(IgnoreApi = true)] // Uncomment when create SDK
[RoleAuthorization(UserRoles.AllAdmins, allowM2M: true)]
public class OpenDataController : BaseV1Controller
{
    public OpenDataController(IConfiguration configuration, ILogger<OpenDataController> logger, AuditLogger auditLogger) 
        : base(configuration, logger, auditLogger)
    {
    }

    [HttpGet("messages/delivered", Name = nameof(GetDeliveredMessagesByYearAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenDataResult))]
    public async Task<IActionResult> GetDeliveredMessagesByYearAsync(
        [FromServices] IRequestClient<GetDeliveredMessagesByYear> client,
        CancellationToken cancellationToken,
        [FromQuery] GetDeliveredMessagesByYearRequest request)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<OpenDataResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Year
                }, cancellationToken));

        return Result(serviceResult);
    }
}
