using eID.PDEAU.API.Authorization;
using eID.PDEAU.API.Requests;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PDEAU.API.Controllers;

/// <summary>
/// Support system of open data
/// </summary>
[RoleAuthorization(allowM2M: true)]
public class OpenDataController : BaseV1Controller
{
    /// <summary>
    /// Crate and instance of the controller
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public OpenDataController(IConfiguration configuration, ILogger<OpenDataController> logger, AuditLogger auditLogger)
        : base(configuration, logger, auditLogger) { }

    /// <summary>
    /// Get all active Providers open data information
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("activeproviders", Name = nameof(GetActiveProvidersAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenDataResult))]
    public async Task<IActionResult> GetActiveProvidersAsync(
        [FromServices] IRequestClient<GetActiveProviders> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<OpenDataResult>>(
                new
                {
                    CorrelationId = RequestId
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get done service by year open data information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("doneservices", Name = nameof(GetDoneServicesByYearAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenDataResult))]
    public async Task<IActionResult> GetDoneServicesByYearAsync(
        [FromServices] IRequestClient<GetDoneServicesByYear> client,
        [FromQuery] GetDoneServicesByYearRequest request,
        CancellationToken cancellationToken)
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
