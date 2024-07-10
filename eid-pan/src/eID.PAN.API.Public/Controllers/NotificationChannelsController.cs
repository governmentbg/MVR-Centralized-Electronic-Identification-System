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
    /// <param name="auditLogger"></param>
    public NotificationChannelsController(IConfiguration configuration, ILogger<NotificationChannelsController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
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

        AddAuditLog(LogEventCode.GetUserNotificationChannelsByFilter);

        return Result(serviceResult);
    }

    /// <summary>
    /// Get user selected notification channels
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("selected")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<Guid>))]
    public async Task<IActionResult> GetSelectedAsync(
        [FromServices] IRequestClient<GetUserNotificationChannels> client,
        [FromServices] IConfiguration configuration,
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

        AddAuditLog(LogEventCode.GetUserNotificationChannels, GetUserId());

        return Result(serviceResult);
    }

    /// <summary>
    /// Register selection of user notification channels
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost("selection")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterSelectedAsync(
        [FromServices] IRequestClient<RegisterUserNotificationChannels> client,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken,
        [FromBody] HashSet<Guid> ids)
    {
        var request = new RegisterUserNotificationChannelsRequest
        {
            Ids = ids
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
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

        AddAuditLog(LogEventCode.RegisterUserNotificationChannels, GetUserId());

        return Result(serviceResult);
    }


    /// <summary>
    /// Add new notification channel in Pending table. Name must be unique against approved channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the Id of the created notification channel.</response>
    /// <returns></returns>
    [HttpPost(Name = nameof(RegisterNotificationChannelAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterNotificationChannelAsync(
        [FromServices] IRequestClient<RegisterNotificationChannel> client,
        [FromBody] RegisterNotificationChannelRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }
        // TODO:
        // Once we have authentication
        // we should verify if SystemId can/should be taken from the token
        // instead of being fed through the request
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId(),
                    request.SystemId,
                    request.Name,
                    request.Description,
                    request.CallbackUrl,
                    request.Price,
                    request.InfoUrl,
                    request.Translations
                }, cancellationToken));

        AddAuditLog(LogEventCode.RegisterNotificationChannel);

        return Result(serviceResult);
    }


    /// <summary>
    /// Update existing approved notification channel. 
    /// New version is added in Pending table. Name must be unique against approved channels.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ModifyAsync(
        [FromServices] IRequestClient<ModifyNotificationChannel> client,
        CancellationToken cancellationToken,
        [FromRoute] string id,
        [FromBody] RegisterNotificationChannelRequest request)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        // TODO:
        // Once we have authentication
        // we should verify that the call is authorized
        // meaning - requesting system is actually parenting the channel
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId(),
                    request.SystemId,
                    request.Name,
                    request.Description,
                    request.CallbackUrl,
                    request.Price,
                    request.InfoUrl,
                    request.Translations
                }, cancellationToken));

        AddAuditLog(LogEventCode.ModifyNotificationChannel);

        return Result(serviceResult);
    }
}
