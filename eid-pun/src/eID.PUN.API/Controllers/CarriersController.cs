using eID.PJS.AuditLogging;
using eID.PUN.API.Requests;
using eID.PUN.Contracts;
using eID.PUN.Contracts.Commands;
using eID.PUN.Contracts.Results;
using MassTransit;
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
    /// <param name="configuration"></param>
    /// <param name="auditLogger"></param>
    public CarriersController(ILogger<CarriersController> logger, IConfiguration configuration, AuditLogger auditLogger) 
        : base(logger, configuration, auditLogger)
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
        var logEventCode = LogEventCode.REGISTER_CARRIER;
        var eId = request.EId.ToString();
        var eventPayload = BeginAuditLog(logEventCode, request, eId,
            (nameof(request.SerialNumber), request.SerialNumber),
            (nameof(request.Type), request.Type),
            (nameof(request.CertificateId), request.CertificateId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, eId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SerialNumber,
                request.Type,
                request.CertificateId,
                request.EId
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload, eId);
    }

    /// <summary>
    /// Gets filtered carrier records when at least one of Type/SerialNumber/Eid/CertificateId is provided.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetByAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CarrierResult>))]
    public async Task<IActionResult> GetByAsync(
        [FromServices] IRequestClient<GetCarriersByFilter> client,
        [FromQuery] GetCarriersByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_CARRIERS_BY;
        var eId = request.EId == Guid.Empty ? null : request.EId.ToString();
        var eventPayload = BeginAuditLog(logEventCode, request, eId,
            (nameof(request.SerialNumber), request.SerialNumber),
            (nameof(request.Type), request.Type),
            (nameof(request.CertificateId), request.CertificateId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, eId);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<CarrierResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.SerialNumber,
                    request.EId,
                    request.CertificateId,
                    request.Type
                }, cancellationToken));

        return ResultWithAuditLogFromCarrierResult(logEventCode, eventPayload, serviceResult, serviceResult.Result, eId);
    }

    private IActionResult ResultWithAuditLogFromCarrierResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IEnumerable<CarrierResult>> serviceResult, IEnumerable<CarrierResult>? data, string? eId)
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
                foreach (var item in data)
                {
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        [nameof(item.SerialNumber)] = item.SerialNumber,
                        [nameof(item.Type)] = item.Type,
                        [nameof(item.CertificateId)] = item.CertificateId
                    };
                    AddAuditLog(logEventCode, suffix, item.EId.ToString(), currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix, eId, eventPayload);
            }
        });
}
