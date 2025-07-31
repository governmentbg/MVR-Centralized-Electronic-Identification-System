using eID.PAN.API.Public.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Public.Controllers;

/// <summary>
/// This controller allows user selects your own notification channels
/// </summary>
public class NotificationChannelsController : BaseV1Controller
{
    /// <summary>
    /// Create instance of <see cref="NotificationChannelsController"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="auditLogger"></param>
    public NotificationChannelsController(ILogger<NotificationChannelsController> logger, IConfiguration configuration, AuditLogger auditLogger)
        : base(logger, configuration, auditLogger)
    {
    }

    /// <summary>
    /// Get all notification channels
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <param name="channelName"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<UserNotificationChannelResult>))]
    public async Task<IActionResult> GetAsync(
        [FromServices] IRequestClient<GetUserNotificationChannelsByFilter> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 100,
        [FromQuery] int pageIndex = 1,
        [FromQuery] string? channelName = null)
    {
        var request = new GetUserNotificationChannelsRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            ChannelName = channelName ?? string.Empty
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<UserNotificationChannelResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    request.ChannelName
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get user selected notification channels
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("selected")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<Guid>))]
    public async Task<IActionResult> GetSelectedAsync(
        [FromServices] IRequestClient<GetUserNotificationChannels> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 1000,
        [FromQuery] int pageIndex = 1)
    {
        var request = new GetSelectedUserNotificationChannelsRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
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

        return Result(serviceResult);
    }

    /// <summary>
    /// Register selection of user notification channels
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost("selection")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterSelectedAsync(
        [FromServices] IRequestClient<RegisterUserNotificationChannels> client,
        [FromBody] HashSet<Guid> ids,
        CancellationToken cancellationToken)
    {
        var request = new RegisterUserNotificationChannelsRequest
        {
            Ids = ids
        };

        var logEventCode = LogEventCode.REGISTER_USER_NOTIFICATION_CHANNELS;
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
    /// Add new notification channel in Pending table. Name must be unique against approved channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the Id of the created notification channel.</response>
    /// <returns></returns>
    [HttpPost(Name = nameof(RegisterAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] IRequestClient<RegisterNotificationChannel> client,
        [FromBody] RegisterNotificationChannelRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.REGISTER_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", GetUserId()));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        if (GetSystemName() == UNKNOWN_SYSTEM)
        {
            AddAuditLog(logEventCode, LogEventLifecycle.FAIL, payload: eventPayload);
            return Unauthorized();
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId(),
                    SystemName = GetSystemName(),
                    request.Name,
                    request.Description,
                    request.CallbackUrl,
                    Price = 0,
                    request.Email,
                    request.InfoUrl,
                    request.Translations
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Update existing approved notification channel. 
    /// New version is added in Pending table. Name must be unique against approved channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ModifyAsync(
        [FromServices] IRequestClient<ModifyNotificationChannel> client,
        [FromRoute] Guid id,
        [FromBody] NotificationChannelPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new ModifyNotificationChannelRequest
        {
            Id = id,
            Name = payload.Name,
            InfoUrl = payload.InfoUrl,
            Description = payload.Description,
            CallbackUrl = payload.CallbackUrl,
            Email = payload.Email,
            Translations = payload.Translations
        };

        var logEventCode = LogEventCode.MODIFY_NOTIFICATION_CHANNEL;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("NotificationChannelId", request.Id),
            ("UserId", GetUserId()));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        if (GetSystemName() == UNKNOWN_SYSTEM)
        {
            AddAuditLog(logEventCode, LogEventLifecycle.FAIL, payload: eventPayload);
            return Unauthorized();
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId(),
                    SystemName = GetSystemName(),
                    request.Name,
                    request.Description,
                    request.CallbackUrl,
                    Price = 0,
                    request.Email,
                    request.InfoUrl,
                    request.Translations
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
