using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

/// <summary>
/// Implement Notification Channels Controller
/// </summary>
public class NotificationChannelsController : BaseV1Controller
{
    /// <summary>
    /// Create an instance of <see cref="NotificationChannelsController"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="auditLogger"></param>
    public NotificationChannelsController(ILogger<NotificationChannelsController> logger, IConfiguration configuration, AuditLogger auditLogger)
        : base(logger, configuration, auditLogger)
    {
    }

    /// <summary>
    /// Returns combined list of all notification channels - Approved, Pending and Rejected
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(INotificationChannelsData<NotificationChannelResult, NotificationChannelRejectedResult>))]
    public async Task<IActionResult> GetAllAsync(
        [FromServices] IRequestClient<GetAllNotificationChannels> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<INotificationChannelsData<NotificationChannelResult, NotificationChannelRejectedResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    UserId = GetUid()
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Approve pending channel by 'Id'.
    /// Channel is moved in Approved table and pending one is removed.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveAsync(
        [FromServices] IRequestClient<ApproveNotificationChannel> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.APPROVE_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    request.Id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUid()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Reject pending channel by 'Id'.
    /// Channel is moved in Rejected table and pending one is removed.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectAsync(
        [FromServices] IRequestClient<RejectNotificationChannel> client,
        [FromRoute] Guid id,
        [FromBody] RejectNotificationChannelPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new RejectNotificationChannelRequest
        {
            Id = id,
            Reason = payload.Reason,
        };

        var logEventCode = LogEventCode.REJECT_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    request.Id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUid(),
                    request.Reason,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Soft delete approved channel by 'Id'.
    /// Channel is moved in Archived table and approved one is removed.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ArchiveAsync(
        [FromServices] IRequestClient<ArchiveNotificationChannel> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.ARCHIVE_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUid()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Restore archived channel by 'Id'.
    /// Channel is moved in Approved table and archived one is removed.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestoreAsync(
        [FromServices] IRequestClient<RestoreNotificationChannel> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.RESTORE_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUid()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Sends test notification to that channel.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}/test")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestHttpCallbackResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> TestAsync(
        [FromServices] IRequestClient<TestNotificationChannel> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.TEST_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<TestHttpCallbackResult>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
