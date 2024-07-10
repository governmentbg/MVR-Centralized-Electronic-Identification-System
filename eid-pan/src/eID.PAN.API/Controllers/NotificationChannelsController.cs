using eID.PJS.AuditLogging;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

public class NotificationChannelsController : BaseV1Controller
{
    public NotificationChannelsController(IConfiguration configuration, ILogger<NotificationChannelsController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Returns combined list of all notification channels - Approved, Pending and Rejected
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(INotificationChannelsData<NotificationChannelResult>))]
    public async Task<IActionResult> GetAllAsync(
        [FromServices] IRequestClient<GetAllNotificationChannels> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<INotificationChannelsData<NotificationChannelResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    UserId = GetUserId()
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetAllNotificationChannels);

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
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId()
                }, cancellationToken));

        AddAuditLog(LogEventCode.ApproveNotificationChannel);

        return Result(serviceResult);
    }

    /// <summary>
    /// Reject pending channel by 'Id'.
    /// Channel is moved in Rejected table and pending one is removed.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectAsync(
        [FromServices] IRequestClient<RejectNotificationChannel> client,
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId()
                }, cancellationToken));

        AddAuditLog(LogEventCode.RejectNotificationChannel);

        return Result(serviceResult);
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
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId()
                }, cancellationToken));

        AddAuditLog(LogEventCode.ArchiveNotificationChannel);

        return Result(serviceResult);
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
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    Id = id,
                    CorrelationId = RequestId,
                    ModifiedBy = GetUserId()
                }, cancellationToken));

        AddAuditLog(LogEventCode.RestoreNotificationChannel);

        return Result(serviceResult);
    }
}
