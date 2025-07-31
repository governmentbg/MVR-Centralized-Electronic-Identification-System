using eID.PIVR.API.Requests;
using eID.PIVR.Contracts;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PIVR.API.Controllers;

/// <summary>
/// Support different type of verifications
/// </summary>
public class VerifyController : BaseV1Controller
{
    public VerifyController(IConfiguration configuration, ILogger<VerifyController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Verifying detached signature
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("signature")]
    public async Task<IActionResult> VerifySignatureAsync(
        [FromServices] IRequestClient<VerifySignature> client,
        [FromBody] VerifySignatureRequest request,
        CancellationToken cancellationToken)
    {
        request ??= new VerifySignatureRequest();

        var logEventCode = LogEventCode.VERIFY_SIGNATURE;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (AuditLoggingKeys.TargetUid, request.Uid),
            (AuditLoggingKeys.TargetUidType, request.UidType.ToString()));
        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.OriginalFile,
                    request.DetachedSignature,
                    request.Uid,
                    request.SignatureProvider,
                    request.UidType
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
