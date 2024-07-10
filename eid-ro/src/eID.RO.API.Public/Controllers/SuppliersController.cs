using eID.PJS.AuditLogging;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace eID.RO.API.Public.Controllers;

/// <summary>
/// Support suppliers of electronic administrative services
/// </summary>
public class SuppliersController : BaseV1Controller
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create an instance of <see cref="SuppliersController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    /// <param name="httpClientFactory"></param>
    public SuppliersController(
        IConfiguration configuration,
        ILogger<SuppliersController> logger,
        AuditLogger auditLogger,
        IHttpClientFactory httpClientFactory)
        : base(configuration, logger, auditLogger)
    {
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PIVR");
    }

    /// <summary>
    /// Get Suppliers of electronic administrative services
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("batches")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<BatchResult>))]
    public async Task<IActionResult> GetBatchesByFilterAsync(
        [FromQuery] GetBatchesByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var parameters = GetKeyValuePairs(request);
        var uri = QueryHelpers.AddQueryString("/api/v1/suppliers/batches", parameters);

        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);

        HttpContext.Response.RegisterForDispose(response);
        
        AddAuditLog(LogEventCode.GetBatchesByFilter);

        return StatusCode((int)response.StatusCode, body);
    }

    /// <summary>
    /// Get supplier services
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("services")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<SupplierServiceResult>))]
    public async Task<IActionResult> GetServicesByFilterAsync(
        [FromQuery] GetServicesByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var parameters = GetKeyValuePairs(request);
        var uri = QueryHelpers.AddQueryString("/api/v1/suppliers/services", parameters);

        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);

        HttpContext.Response.RegisterForDispose(response);

        AddAuditLog(LogEventCode.GetServicesByFilter);

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
        var response = await _httpClient.GetAsync($"/api/v1/suppliers/services/{serviceId}/scope", cancellationToken);
        var body = await response.Content.ReadAsStreamAsync(cancellationToken);

        HttpContext.Response.RegisterForDispose(response);

        AddAuditLog(LogEventCode.GetAllServiceScopes);

        return StatusCode((int)response.StatusCode, body);
    }

    private static Dictionary<string, string?> GetKeyValuePairs(object o) =>
        o.GetType().GetProperties()
            .Where(p => p.GetValue(o) != null)
            .ToDictionary(k => k.Name, v => v.GetValue(o)?.ToString());
}
