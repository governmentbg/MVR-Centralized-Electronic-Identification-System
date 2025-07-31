using eID.PJS.AuditLogging;
using eID.Signing.API.Public.Requests;
using eID.Signing.Contracts;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.Signing.API.Public.Controllers;

/// <summary>
/// Provides access to online signing services of Evrotrust
/// </summary>
public class EvrotrustController : BaseV1Controller
{
    /// <summary>
    /// Crate an instance of <see cref="EvrotrustController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public EvrotrustController(IConfiguration configuration, ILogger<EvrotrustController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    { }

    /// <summary>
    /// Check for signing status of a file by TransactionId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="transactionId"></param>
    /// <param name="groupSigning"></param>
    /// <returns></returns>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EvrotrustStatusResult))]
    public async Task<IActionResult> GetFileStatusAsync(
        [FromServices] IRequestClient<EvrotrustGetFileStatusByTransactionId> client,
        CancellationToken cancellationToken,
        [FromQuery] string transactionId,
        [FromQuery] bool groupSigning = false)
    {
        var request = new EvrotrustGetFileByTransactionIdRequest
        {
            TransactionId = transactionId,
            GroupSigning = groupSigning
        };

        var logEventCode = LogEventCode.EVROTRUST_GET_FILE_STATUS;
        var eventPayload = BeginAuditLog(logEventCode, request, (nameof(request.TransactionId), request.TransactionId));

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<EvrotrustStatusResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.TransactionId,
                    request.GroupSigning
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Downloads the content of a signed file by TransactionId
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="transactionId"></param>
    /// <param name="groupSigning"></param>
    /// <returns></returns>
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EvrotrustFileContent>))]
    public async Task<IActionResult> DownloadFileAsync(
        [FromServices] IRequestClient<EvrotrustDownloadFileByTransactionId> client,
        CancellationToken cancellationToken,
        [FromQuery] string transactionId,
        [FromQuery] bool groupSigning = false)
    {
        var request = new EvrotrustGetFileByTransactionIdRequest
        {
            TransactionId = transactionId,
            GroupSigning = groupSigning
        };

        var logEventCode = LogEventCode.EVROTRUST_DOWNLOAD_FILE;
        var eventPayload = BeginAuditLog(logEventCode, request, (nameof(request.TransactionId), request.TransactionId));

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<EvrotrustFileContent>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.TransactionId,
                    request.GroupSigning
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Send file content for online signing
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>TransactionId for future verification and checks</returns>
    [HttpPost("sign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EvrotrustDocumentSigningResult))]
    public async Task<IActionResult> SignDocumentAsync(
        [FromServices] IRequestClient<EvrotrustSignDocument> client,
        CancellationToken cancellationToken,
        [FromBody] EvrotrustSignDocumentRequest request)
    {
        var logEventCode = LogEventCode.EVROTRUST_SIGN_DOCUMENT;
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
            client.GetResponse<ServiceResult<EvrotrustDocumentSigningResult>>(new
            {
                CorrelationId = RequestId,
                request.DateExpire,
                request.Documents,
                UserIdentifiers = new string[] { request.Uid }
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Checks the status of the logged-in user in Evrotrust system by Uid
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("user/check")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EvrotrustUserCheckResult))]
    public async Task<IActionResult> CheckUserAsync(
        [FromServices] IRequestClient<EvrotrustCheckUserByUid> client,
        [FromBody] UserCheckRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.EVROTRUST_CHECK_USER;
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
            client.GetResponse<ServiceResult<EvrotrustUserCheckResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Uid
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
