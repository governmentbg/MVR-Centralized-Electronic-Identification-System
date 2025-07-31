using eID.PJS.AuditLogging;
using eID.Signing.API.Public.Requests;
using eID.Signing.Contracts;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.Signing.API.Public.Controllers;

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

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaFileStatusResult))]
    public async Task<IActionResult> GetFileStatusAsync(
        [FromServices] IRequestClient<BoricaGetFileStatusByTransactionId> client,
        CancellationToken cancellationToken,
        [FromQuery] string transactionId)
    {
        var request = new BoricaGetFileByTransactionIdRequest
        {
            TransactionId = transactionId
        };

        var logEventCode = LogEventCode.BORICA_GET_FILE_STATUS;
        var eventPayload = BeginAuditLog(logEventCode, request, (nameof(request.TransactionId), request.TransactionId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaFileStatusResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.TransactionId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaFileContent))]
    public async Task<IActionResult> DownloadFileAsync(
        [FromServices] IRequestClient<BoricaDownloadFileByTransactionId> client,
        CancellationToken cancellationToken,
        [FromQuery] string transactionId)
    {
        var request = new BoricaGetFileByTransactionIdRequest
        {
            TransactionId = transactionId
        };

        var logEventCode = LogEventCode.BORICA_DOWNLOAD_FILE;
        var eventPayload = BeginAuditLog(logEventCode, request, (nameof(request.TransactionId), request.TransactionId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaFileContent>>(
                new
                {
                    CorrelationId = RequestId,
                    request.TransactionId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("sign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaDocumentSigningResult))]
    public async Task<IActionResult> SignDocumentAsync(
        [FromServices] IRequestClient<BoricaSignDocument> client,
        CancellationToken cancellationToken,
        [FromBody] BoricaSignDocumentRequest request)
    {
        var logEventCode = LogEventCode.BORICA_SIGN_DOCUMENT;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!string.IsNullOrWhiteSpace(GetUid()))
        {
            Logger.LogInformation("Overriding Uid with what's available in the token for request with correlation id {RequestId}", RequestId);
            request.Uid = GetUid();
        }
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaDocumentSigningResult>>(new
            {
                CorrelationId = RequestId,
                request.Uid,
                request.Contents
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    [HttpPost("user/check")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoricaCertificateCheckResult))]
    public async Task<IActionResult> CheckUserAsync(
        [FromServices] IRequestClient<BoricaCheckUserByUid> client,
        [FromBody] UserCheckRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.BORICA_CHECK_USER;
        var eventPayload = BeginAuditLog(logEventCode, request);
        if (!string.IsNullOrWhiteSpace(GetUid()))
        {
            Logger.LogInformation("Overriding Uid with what's available in the token for request with correlation id {RequestId}", RequestId);
            request.Uid = GetUid();
        }
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<BoricaCertificateCheckResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
