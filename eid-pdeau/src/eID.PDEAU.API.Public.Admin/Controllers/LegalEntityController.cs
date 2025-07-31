using System.Net;
using eID.PDEAU.API.Public.Admin.Requests;
using eID.PDEAU.API.Public.Admin.Responses;
using eID.PDEAU.Contracts;
using eID.PDEAU.Service.Responses;
using eID.PJS.AuditLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace eID.PDEAU.API.Public.Admin.Controllers;

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
    /// This endpoint will return information about the given UIC (EIK) from TR/Bulstat
    /// Information about the legal entity retrieved from TR (isFoundInRedix = true => ActualStateResponseV3) or Bulstat (isFoundInRedix = false => StateOfPlayResponseType)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Information about the legal entity retrieved from TR (isFoundInRedix = true => ActualStateResponseV3) or Bulstat (isFoundInRedix = false => StateOfPlayResponseType)</returns>
    [HttpGet("info", Name = nameof(GetLegalEntityInfoAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLegalEntityInfoAsync(
        [FromQuery] GetLegalEntityInfoRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_LEGAL_ENTITY_INFO;
        var eventPayload = BeginAuditLog(logEventCode, request, (nameof(request.UIC), request.UIC));

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var queryString = new Dictionary<string, string?>()
        {
            { "UIC", request.UIC }
        };
        //Check if UIC exists in Regix (/api/v1/Registries/tr/getactualstatev3)
        var uri = QueryHelpers.AddQueryString("/api/v1/Registries/tr/getactualstatev3", queryString);
        SetRequestIdDefaultHeader(_httpClient);
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var regixResponse = JsonConvert.DeserializeObject<RegiXSearchResult<ActualStateResponseV3>>(content);
        var isFoundInRedix = regixResponse?.Response?.Values?.FirstOrDefault(x => x?.Deed?.UIC == request.UIC);
        //If UIC not found in Regix, check in Bulstat
        if (isFoundInRedix == null)
        {
            uri = QueryHelpers.AddQueryString("/api/v1/Registries/bulstat/getstateofplay", queryString);
            response = await _httpClient.GetAsync(uri, cancellationToken);
            content = await response.Content.ReadAsStringAsync(cancellationToken);
        }
        HttpContext.Response.RegisterForDispose(response);

        // < 400 OK
        if (response.StatusCode < HttpStatusCode.BadRequest)
        {
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload);
        }
        else // Error
        {
            eventPayload["Reason"] = response.ToString();
            eventPayload["ResponseStatusCode"] = response.StatusCode;
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
        }

        //IsFoundInRegix is added to know how to parse the result
        return StatusCode((int)response.StatusCode, new
        {
            OriginalResponse = JsonConvert.DeserializeObject<object>(content),
            IsFoundInRegix = isFoundInRedix != null
        });
    }
}
