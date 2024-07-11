using eID.PJS.AuditLogging;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

public class CommunicationsController : BaseV1Controller
{
    public CommunicationsController(IConfiguration configuration, ILogger<CommunicationsController> logger, AuditLogger auditLogger) 
        : base(configuration, logger, auditLogger)
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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        await publishEndpoint.Publish<SendEmail>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(LogEventCode.SendEmail, request.UserId.ToString());

        return Accepted();
    }

    /// <summary>
    /// Queue direct email for sending
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("direct-emails/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendDirectEmailAsync(
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromBody] SendDirectEmailRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
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

        AddAuditLog(LogEventCode.SendDirectEmail, GetUserId());

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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        await publishEndpoint.Publish<SendSms>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(LogEventCode.SendSms, request.UserId.ToString());

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
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        await publishEndpoint.Publish<SendPushNotification>(new
        {
            CorrelationId = RequestId,
            request.UserId,
            request.Translations
        }, cancellationToken);

        AddAuditLog(LogEventCode.SendPushNotification, request.UserId.ToString());

        return Accepted();
    }
}
