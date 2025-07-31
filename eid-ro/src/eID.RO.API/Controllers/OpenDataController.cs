using eID.PJS.AuditLogging;
using eID.RO.API.Authorization;
using eID.RO.API.Requests;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.RO.API.Controllers;

/// <summary>
/// Support system of open data
/// </summary>
//[ApiExplorerSettings(IgnoreApi = true)] // Uncomment when create SDK
[RoleAuthorization(UserRoles.AppRUAndCISAdmins, allowM2M: true)]
public class OpenDataController : BaseV1Controller
{
    public OpenDataController(IConfiguration configuration, ILogger<OpenDataController> logger, AuditLogger auditLogger) 
        : base(configuration, logger, auditLogger)
    {
    }

    [HttpGet("empowerments/activated", Name = nameof(GetActivatedEmpowermentsByYearAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenDataResult))]
    public async Task<IActionResult> GetActivatedEmpowermentsByYearAsync(
        [FromServices] IRequestClient<GetActivatedEmpowermentsByYear> client,
        CancellationToken cancellationToken,
        [FromQuery] GetActivatedEmpowermentsByYearRequest request)
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
