using eID.PJS.AuditLogging;
using eID.PUN.API.Requests;
using eID.PUN.Contracts;
using eID.PUN.Contracts.Commands;
using eID.PUN.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eID.PUN.API.Controllers;

/// <summary>
/// This controller registers and gets eId Carriers. 
/// It will be mostly used by other internal/external systems.
/// </summary>
public class CarriersController : BaseV1Controller
{
    /// <summary>
    /// Create instance of <see cref="CarriersController"/>
    /// </summary>
    /// <param name="logger"></param>
    public CarriersController(IConfiguration configuration, ILogger<CarriersController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Creates a new carrier record
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of created carrier</returns>
    [HttpPost(Name = nameof(RegisterCarrierAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    public async Task<IActionResult> RegisterCarrierAsync(
        [FromServices] IRequestClient<RegisterCarrier> client,
        [FromBody] RegisterCarrierRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                SerialNumber = request.SerialNumber,
                Type = request.Type,
                CertificateId = request.CertificateId,
                EId = request.EId,
                UserId = Guid.NewGuid(),//TODO: get real id from Token later
                ModifiedBy = GetUserId()
            }, cancellationToken));

        AddAuditLog(LogEventCode.RegisterCarrier, request.EId.ToString());

        return Result(serviceResult);
    }

    /// <summary>
    /// Gets filtered carrier records when at least one of SerialNumber/Eid/CertificateId is provided.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetByAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CarrierResult>))]
    public async Task<IActionResult> GetByAsync(
        [FromServices] IRequestClient<GetCarriersBy> client,
        CancellationToken cancellationToken,
        [FromQuery] GetCarriersByFilter filter)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<CarrierResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    filter.SerialNumber,
                    filter.EId,
                    filter.CertificateId
                }, cancellationToken));

        if (serviceResult.Result?.Any() == true)
        {
            foreach (var eID in serviceResult.Result.Select(r => r.EId).Distinct())
            {
                AddAuditLog(LogEventCode.GetCarriersBy, eID.ToString());
            }
        }

        return Result(serviceResult);
    }
}
