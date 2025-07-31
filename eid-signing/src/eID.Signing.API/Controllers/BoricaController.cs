using eID.PJS.AuditLogging;
using eID.Signing.API.Requests;
using eID.Signing.Contracts;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using eID.Signing.Service.Entity;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eID.Signing.API.Controllers;

/// <summary>
/// Provides access to online signing services of Borica
/// </summary>
public class BoricaController : BaseV1Controller
{
    /// <summary>
    /// Crate an instance of <see cref="BoricaController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public BoricaController(IConfiguration configuration, ILogger<BoricaController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    { }

    [HttpPost("consents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaSendConsentResult))]
    public async Task<IActionResult> SendConsentAsync(
        [FromServices] IRequestClient<BoricaSendConsent> client,
        [FromBody] BoricaSendConsentRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.BORICA_SEND_CONSENT;
        var eventPayload = BeginAuditLog(logEventCode, request);


        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaSendConsentResult>>(new
            {
                CorrelationId = RequestId,
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpGet("accesstokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessTokenResult))]
    public async Task<IActionResult> GetAccessTokensAsync(
        [FromServices] IRequestClient<BoricaGetAccessTokens> client,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.BORICA_GET_ACCESS_TOKENS;
        var eventPayload = BeginAuditLog(logEventCode);

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<AccessTokenResult>>>(new
            {
                CorrelationId = RequestId
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("accesstokens")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    public async Task<IActionResult> AddAccessTokenAsync(
        [FromServices] IRequestClient<BoricaAddAccessToken> client,
        [FromBody] BoricaAddAccessTokenRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.BORICA_ADD_ACCESS_TOKEN;
        var eventPayload = BeginAuditLog(logEventCode);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<int>>(new
            {
                CorrelationId = RequestId,
                request.AccessTokenValue,
                request.ExpirationDate
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpGet("consents/{callbackId}/check")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaCheckConsentStatusResponse))]
    public async Task<IActionResult> CheckConsentStatusAsync(
        [FromServices] IRequestClient<BoricaCheckConsentStatus> client,
        [FromRoute] string callbackId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.BORICA_CHECK_CONSENT_STATUS;
        var eventPayload = BeginAuditLog(logEventCode);

        if (string.IsNullOrWhiteSpace(callbackId))
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("CallbackId", "CallbackId is required");
            eventPayload.Add("Reason", "CallbackId is required");
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaCheckConsentStatusResponse>>(new
            {
                CorrelationId = RequestId,
                CallbackId = callbackId
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("sign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ARSSyncSignedContentsResponse))]
    public async Task<IActionResult> ARSSignDocumentAsync(
        [FromServices] IRequestClient<BoricaARSSignDocument> client,
        CancellationToken cancellationToken,
        [FromBody] ARSBoricaSignDocumentRequest request)
    {
        var logEventCode = LogEventCode.BORICA_ASRSIGN_DOCUMENT;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<ARSSyncSignedContentsResponse>>(new
            {
                CorrelationId = RequestId,
                request.Contents
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
