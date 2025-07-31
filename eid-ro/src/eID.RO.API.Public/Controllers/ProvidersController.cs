using eID.PJS.AuditLogging;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace eID.RO.API.Public.Controllers;

public class ProvidersController : BaseV1Controller
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create an instance of <see cref="ProvidersController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    /// <param name="httpClientFactory"></param>
    public ProvidersController(
        IConfiguration configuration,
        ILogger<ProvidersController> logger,
        AuditLogger auditLogger,
        IHttpClientFactory httpClientFactory)
        : base(configuration, logger, auditLogger)
    {
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PDEAU");
    }

    /// <summary>
    /// Get providers list
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderListResult>))]
    public async Task<IActionResult> GetProvidersListByFilterAsync(
                [FromQuery] GetProviderDetailsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var parameters = GetKeyValuePairs(request);
        var uri = QueryHelpers.AddQueryString("/api/v1/providers/list", parameters);
        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);
        HttpContext.Response.RegisterForDispose(response);

        return StatusCode((int)response.StatusCode, body);
    }

    /// <summary>
    /// Get provider services
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("services")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderServiceResult>))]
    public async Task<IActionResult> GetServicesByFilterAsync(
        [FromQuery] GetServicesByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var parameters = GetKeyValuePairs(request);
        parameters.TryAdd("IncludeInactive", false.ToString());
        parameters.TryAdd("IncludeApprovedOnly", true.ToString());

        var uri = QueryHelpers.AddQueryString("/api/v1/providers/services", parameters);
        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);

        HttpContext.Response.RegisterForDispose(response);

        return StatusCode((int)response.StatusCode, body);
    }

    /// <summary>
    /// Get all scopes per service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    [HttpGet("services/{serviceId}/scope")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServiceScopeResult>))]
    public async Task<IActionResult> GetAllServiceScopesAsync(
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.GetAsync($"/api/v1/providers/services/{serviceId}/scope", cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);

        HttpContext.Response.RegisterForDispose(response);

        return StatusCode((int)response.StatusCode, body);
    }

    private static Dictionary<string, string?> GetKeyValuePairs(object o) =>
        o.GetType().GetProperties()
            .Where(p => p.GetValue(o) != null)
            .ToDictionary(k => k.Name, v => v.GetValue(o)?.ToString());
}
