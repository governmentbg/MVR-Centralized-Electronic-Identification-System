using eID.PAN.API.Public.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Public.Controllers;

/// <summary>
/// This controller allows user disables notifications
/// </summary>
public class NotificationsController : BaseV1Controller
{
    /// <summary>
    /// Create an instance of <see cref="NotificationsController"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="auditLogger"></param>
    public NotificationsController(ILogger<NotificationsController> logger, IConfiguration configuration, AuditLogger auditLogger) 
        : base(logger, configuration, auditLogger)
    {
    }

    /// <summary>
    /// Get all Systems and notifications
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <param name="systemName"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<RegisteredSystemResult>))]
    public async Task<IActionResult> GetAsync(
        [FromServices] IRequestClient<GetSystemsAndNotificationsByFilter> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageIndex = 1,
        [FromQuery] string? systemName = null)
    {
        var request = new GetUserNotificationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SystemName = systemName ?? string.Empty
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<RegisteredSystemResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    request.SystemName
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get deactivated user notifications
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("deactivated")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<Guid>))]
    public async Task<IActionResult> GetDeactivatedAsync(
        [FromServices] IRequestClient<GetDeactivatedUserNotifications> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 1000,
        [FromQuery] int pageIndex = 1)
    {
        var request = new GetDeactivatedUserNotificationsRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var logEventCode = LogEventCode.GET_DEACTIVATED_USER_NOTIFICATIONS;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", GetUserId()));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<Guid>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    UserId = GetUserId()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Register deactivated user events
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost("deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterDeactivatedEventsAsync(
        [FromServices] IRequestClient<RegisterDeactivatedEvents> client,
        [FromBody] HashSet<Guid> ids,
        CancellationToken cancellationToken)
    {
        var request = new RegisterDeactivatedEventsRequest
        {
            Ids = ids
        };

        var logEventCode = LogEventCode.DEACTIVATE_USER_NOTIFICATIONS;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", GetUserId()));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    UserId = GetUserId(),
                    request.Ids,
                    ModifiedBy = GetUserId()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Register or update a system with its events.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns id of the newly registered or updated system</response>
    /// <returns></returns>
    [HttpPost("systems", Name = nameof(RegisterOrUpdateSystemAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOrUpdateSystemAsync(
        [FromServices] IRequestClient<RegisterSystem> client,
        [FromBody] RegisterSystemRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.REGISTER_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", GetUserId()),
            (nameof(request.SystemName), request.SystemName));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SystemName,
                ModifiedBy = GetUserId(),
                Translations = (IEnumerable<RegisteredSystemTranslation>)request.Translations,
                Events = (IEnumerable<SystemEvent>)request.Events
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Send notification to a user via users' selected channels or fallback to default(SMTP) channel.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">Returns true if the system managed to queue the notification for sending</response>
    /// <returns>Whether or not the system managed to queue the notification for sending</returns>
    [HttpPost("send", Name = nameof(SendNotificationAsync))]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendNotificationAsync(
        [FromServices] IRequestClient<SendNotification> client,
        [FromBody] SendNotificationRequestInput input,
        CancellationToken cancellationToken)
    {
        var request = new SendNotificationRequest
        {
            UserId = input.UserId,
            EId = input.EId,
            Uid = input.Uid ?? string.Empty,
            UidType = input.UidType,
            EventCode = input.EventCode,
            SystemName = GetSystemName(),
        };

        var logEventCode = LogEventCode.SEND_NOTIFICATION;
        var eventPayload = BeginAuditLog(logEventCode, request, request.EId.ToString(),
            (nameof(request.UserId), request.UserId ?? Guid.Empty),
            (AuditLoggingKeys.TargetUid, request.Uid),
            (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
            (nameof(request.EventCode), request.EventCode),
            (nameof(request.SystemName), request.SystemName));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<bool>>(new
            {
                CorrelationId = RequestId,
                request.SystemName,
                request.EventCode,
                request.UserId,
                request.EId,
                request.Uid,
                Translations = Array.Empty<Contracts.Commands.SendNotificationTranslation>(),
                request.UidType
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
