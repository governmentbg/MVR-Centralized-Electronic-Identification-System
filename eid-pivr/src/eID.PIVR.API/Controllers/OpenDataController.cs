using eID.PIVR.API.Requests;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PIVR.API.Controllers;

/// <summary>
/// Support system of open data
/// </summary>
public class OpenDataController : BaseV1Controller
{
    public OpenDataController(IConfiguration configuration, ILogger<OpenDataController> logger, AuditLogger auditLogger)
        : base(configuration, logger, auditLogger)
    {
    }

    [HttpGet("registries/access-stats", Name = nameof(GetApiUsageByYearAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenDataResult))]
    public async Task<IActionResult> GetApiUsageByYearAsync(
        [FromServices] IRequestClient<GetApiUsageByYear> client,
        CancellationToken cancellationToken,
        [FromQuery] GetApiUsageByYearRequest request)
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
