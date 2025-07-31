using eID.PJS.AuditLogging;
using eID.Signing.API.Public.Requests;
using eID.Signing.Contracts;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.Signing.API.Public.Controllers;

/// <summary>
/// Provides access to offline signing with KEP
/// </summary>
public class KEPController : BaseV1Controller
{
    /// <summary>
    /// Crate an instance of <see cref="KEPController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public KEPController(IConfiguration configuration, ILogger<KEPController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    { }

    [HttpPost("data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KEPDataToSignResult))]
    public async Task<IActionResult> GetDataToSignAsync(
        [FromServices] IRequestClient<KEPGetDataToSign> client,
        CancellationToken cancellationToken,
        [FromBody] KEPGetDataRequest request)
    {
        var logEventCode = LogEventCode.KEP_GET_DATA_TO_SIGN;
        var eventPayload = BeginAuditLog(logEventCode, request);

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<KEPDataToSignResult>>(new
            {
                CorrelationId = RequestId,
                request.DocumentToSign,
                request.SigningCertificate,
                request.CertificateChain,
                request.EncryptionAlgorithm, //check if FE will switch it, or will send just one option
                request.SigningDate
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("sign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KEPSignedDocumentResult))]
    public async Task<IActionResult> SignDataAsync(
        [FromServices] IRequestClient<KEPSignData> client,
        CancellationToken cancellationToken,
        [FromBody] KEPSignDataRequest request)
    {
        var logEventCode = LogEventCode.KEP_SIGN_DATA;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<KEPSignedDocumentResult>>(new
            {
                CorrelationId = RequestId,
                request.DocumentToSign,
                request.SignatureValue,
                request.SigningCertificate,
                request.CertificateChain,
                request.EncryptionAlgorithm, //check if FE will switch it, or will send just one option
                request.SigningDate
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("digest/data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KEPDataToSignResult))]
    public async Task<IActionResult> GetDigestToSignAsync(
        [FromServices] IRequestClient<KEPGetDigestToSign> client,
        CancellationToken cancellationToken,
        [FromBody] KEPGetDigestRequest request)
    {
        var logEventCode = LogEventCode.KEP_GET_DATA_TO_SIGN;
        var eventPayload = BeginAuditLog(logEventCode, request);

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<KEPDataToSignResult>>(new
            {
                CorrelationId = RequestId,
                request.DigestToSign,
                request.DocumentName,
                request.SigningCertificate,
                request.CertificateChain,
                request.EncryptionAlgorithm, //check if FE will switch it, or will send just one option
                request.SigningDate
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("digest/sign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KEPSignedDocumentResult))]
    public async Task<IActionResult> SignDigestAsync(
        [FromServices] IRequestClient<KEPSignDigest> client,
        CancellationToken cancellationToken,
        [FromBody] KEPSignDigestRequest request)
    {
        var logEventCode = LogEventCode.BORICA_SIGN_DOCUMENT;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<KEPSignedDocumentResult>>(new
            {
                CorrelationId = RequestId,
                request.DigestToSign,
                request.DocumentName,
                request.SignatureValue,
                request.SigningCertificate,
                request.CertificateChain,
                request.EncryptionAlgorithm, //check if FE will switch it, or will send just one option
                request.SigningDate
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
