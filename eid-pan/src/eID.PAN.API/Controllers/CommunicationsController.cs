using eID.PAN.API.Authorization;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

public class CommunicationsController : BaseV1Controller
{
    public CommunicationsController(ILogger<CommunicationsController> logger, IConfiguration configuration, AuditLogger auditLogger)
        : base(logger, configuration, auditLogger)
    {
    }

    /// <summary>
    /// Queue email for sending
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("emails/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendEmailAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromBody] SendEmailRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_EMAIL;
        var eventPayload = BeginAuditLog(logEventCode, request, request.UserId.ToString());

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, request.UserId.ToString());
        }

        await publishEndpoint.Publish<SendEmail>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(logEventCode, LogEventLifecycle.SUCCESS, request.UserId.ToString(), eventPayload);

        return Accepted();
    }

    /// <summary>
    /// Queue direct email for sending
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RoleAuthorization(allowM2M: true)]
    [HttpPost("direct-emails/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendDirectEmailAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromBody] SendDirectEmailRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_DIRECT_EMAIL;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        //TODO: Think about adding sender information in command for log purposes
        await publishEndpoint.Publish<SendDirectEmail>(new
        {
            CorrelationId = RequestId,
            request.Language,
            request.Subject,
            request.Body,
            request.EmailAddress
        }, cancellationToken);

        AddAuditLog(logEventCode, LogEventLifecycle.SUCCESS, payload: eventPayload);

        return Accepted();
    }

    /// <summary>
    /// Queue sms for sending
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("sms/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendSmsAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromBody] SendSmsRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_SMS;
        var eventPayload = BeginAuditLog(logEventCode, request, request.UserId.ToString());

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, request.UserId.ToString());
        }

        await publishEndpoint.Publish<SendSms>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(logEventCode, LogEventLifecycle.SUCCESS, request.UserId.ToString(), eventPayload);

        return Accepted();
    }

    /// <summary>
    /// Queue push notifications for sending
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("pushNotification/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendPushNotificationAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromBody] SendPushNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_PUSH_NOTIFICATION;
        var eventPayload = BeginAuditLog(logEventCode, request, request.UserId.ToString());

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload, request.UserId.ToString());
        }

        await publishEndpoint.Publish<SendPushNotification>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(logEventCode, LogEventLifecycle.SUCCESS, request.UserId.ToString(), eventPayload);

        return Accepted();
    }
}
