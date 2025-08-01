using eID.PDEAU.API.Public.Requests;
using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PDEAU.API.Public.Controllers;

/// <summary>
/// Support provider details
/// </summary>
public partial class ProvidersController : BaseV1Controller
{
    /// <summary>
    /// Get providers of electronic administrative services
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderDetailsResult>))]
    public async Task<IActionResult> GetProviderDetailsByFilterAsync(
        [FromServices] IRequestClient<GetProviderDetailsByFilter> client,
        [FromQuery] GetProviderDetailsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PROVIDER_DETAILS_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<ProviderDetailsResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    request.Name,
                    request.Status,
                    request.IncludeDeleted,
                    request.IncludeWithServicesOnly,
                    request.IncludeEmpowermentOnly
                }, cancellationToken));

        return ResultWithAuditLogFromProviderDetailResult(logEventCode, eventPayload, serviceResult, serviceResult.Result?.Data);
    }
    /// <summary>
    /// Get providers of electronic administrative services
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("details/available")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProviderDetailsResult>))]
    public async Task<IActionResult> GetAvailableProviderDetailsByFilterAsync(
        [FromServices] IRequestClient<GetAvailableProviderDetailsByFilter> client,
        [FromQuery] GetAvailableProviderDetailsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_AVAILABLE_PROVIDER_DETAILS_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ProviderDetailsResult>>>(
                new
                {
                    CorrelationId = RequestId
                }, cancellationToken));

        return ResultWithAuditLogFromProviderDetailResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Get single provider details
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("details/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderDetailsResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderDetailsByIdAsync(
        [FromServices] IRequestClient<GetProviderDetailsById> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.GET_PROVIDER_DETAILS_BY_ID;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("ProvidersDetailsId", id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<ProviderDetailsResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return ResultWithAuditLogFromProviderDetailResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Set <paramref name="status"/> for provider details <paramref name="id"/> 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id">Provider details id</param>
    /// <param name="status"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("details/{id}/{status}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetProviderDetailsStatusAsync(
        [FromServices] IRequestClient<SetProviderDetailsStatus> client,
        [FromRoute] Guid id,
        [FromRoute] ProviderDetailsStatusType status,
        CancellationToken cancellationToken)
    {
        var request = new SetProviderDetailStatusRequest { Id = id, Status = status };

        var logEventCode = LogEventCode.SET_PROVIDER_DETAILS_STATUS;
        var eventPayload = BeginAuditLog(logEventCode,
                                         request,
                                         ("ProvidersDetailsId", request.Id),
                                         ("Status", request.Status.ToString()));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    request.Status
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get providers sections
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("sections")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<SectionResult>))]
    public async Task<IActionResult> GetSectionsByFilterAsync(
        [FromServices] IRequestClient<GetSectionsByFilter> client,
        [FromQuery] GetSectionsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<SectionResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageIndex,
                    request.PageSize,
                    request.Name,
                    request.IncludeDeleted
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get provider section
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("sections/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSectionByIdAsync(
        [FromServices] IRequestClient<GetSectionById> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<SectionResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get a service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("services/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderServiceResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceByIdAsync(
        [FromServices] IRequestClient<GetServiceById> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<ProviderServiceResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get all scopes per service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    [HttpGet("services/{serviceId}/scope")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServiceScopeResult>))]
    public async Task<IActionResult> GetAllServiceScopesAsync(
        [FromServices] IRequestClient<GetAllServiceScopes> client,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var request = new GetAllServiceScopesRequest
        {
            ServiceId = serviceId,
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ServiceScopeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ServiceId,
                    ProviderId = Guid.Empty
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get default service scopes
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("services/scope/defaults")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> GetDefaultServiceScopesAsync(
        [FromServices] IRequestClient<GetDefaultServiceScopes> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<string>>>(
                new
                {
                    CorrelationId = RequestId
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get collection of names already used by your provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("services/scope/available")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> GetAvailableScopesByProviderIdAsync(
        [FromServices] IRequestClient<GetAvailableScopesByProviderId> client,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(GetSystemId(), out var providerId);
        var request = new GetByIdRequest { Id = providerId };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<string>>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    private IActionResult ResultWithAuditLogFromProviderDetailResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<ProviderDetailsResult> serviceResult, ProviderDetailsResult? data)
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
            if (data is not null && !data.SyncedFromOnlineRegistry)
            {

                var currentPayload = new SortedDictionary<string, object>(eventPayload)
                {
                    [nameof(data.UIC)] = data.UIC,
                    [nameof(data.Name)] = data.Name,
                };
                AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });

    private IActionResult ResultWithAuditLogFromProviderDetailResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IPaginatedData<ProviderDetailsResult>> serviceResult, IEnumerable<ProviderDetailsResult>? data)
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
                foreach (var uic in data.Where(d => !d.SyncedFromOnlineRegistry).Select(r => r.UIC).Distinct())
                {
                    var provider = data.First(r => r.UIC == uic);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        ["ProviderId"] = provider.Id
                    };
                    AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });

    private IActionResult ResultWithAuditLogFromProviderDetailResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IEnumerable<ProviderDetailsResult>> serviceResult, IEnumerable<ProviderDetailsResult>? data)
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
                foreach (var uic in data.Where(d => !d.SyncedFromOnlineRegistry).Select(r => r.UIC).Distinct())
                {
                    var provider = data.First(r => r.UIC == uic);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        ["ProviderId"] = provider.Id
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
