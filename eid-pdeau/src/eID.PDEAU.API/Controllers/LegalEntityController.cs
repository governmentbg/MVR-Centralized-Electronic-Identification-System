using eID.PDEAU.API.Responses;
using eID.PJS.AuditLogging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eID.PDEAU.API.Controllers;

public class LegalEntityController : BaseV1Controller
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create an instance of <see cref="LegalEntityController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    /// <param name="httpClientFactory"></param>
    public LegalEntityController(
        IConfiguration configuration,
        ILogger<LegalEntityController> logger,
        AuditLogger auditLogger,
        IHttpClientFactory httpClientFactory)
        : base(configuration, logger, auditLogger)
    {
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PIVR");
    }

    /// <summary>
    /// This endpoint will return nomenclatures from Bulstat
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Extraction of the existing nomenclatures from Bulstat</returns>
    [HttpGet("nomenclatures", Name = nameof(GetBulstatNomenclaturesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBulstatNomenclaturesAsync(
        CancellationToken cancellationToken)
    {
        var uri = "/api/v1/Registries/bulstat/fetchnomenclatures";
        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var regixResponse = JsonConvert.DeserializeObject<RegiXSearchResult<FetchNomenclaturesResponseType>>(content);
        HttpContext.Response.RegisterForDispose(response);

        return StatusCode((int)response.StatusCode, regixResponse);
    }
}
