using eID.PAN.API.Authorization;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Enums;
using eID.PAN.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

/// <summary>
/// Implement <see cref="NotificationsController"/>
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
    /// Register or update a system with its events.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns id of the newly registered or updated system</response>
    /// <returns></returns>
    [RoleAuthorization(allowM2M: true)]
    [HttpPost("systems", Name = nameof(RegisterOrUpdateSystemAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOrUpdateSystemAsync(
        [FromServices] IRequestClient<RegisterSystem> client,
        [FromBody] RegisterSystemRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.REGISTER_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                SystemName = GetSystemName(),
                ModifiedBy = GetUid(),
                Translations = (IEnumerable<RegisteredSystemTranslation>)request.Translations,
                Events = (IEnumerable<SystemEvent>)request.Events
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get all registered systems
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <param name="systemName"></param>
    /// <param name="includeDeleted"></param>
    /// <returns></returns>
    [HttpGet("systems/registered")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<RegisteredSystemResult>))]
    public async Task<IActionResult> GetNotificationsByFilterAsync(
        [FromServices] IRequestClient<GetNotificationsByFilter> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1,
        [FromQuery] string? systemName = null,
        [FromQuery] bool? includeDeleted = null)
    {
        var request = new GetNotificationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SystemName = systemName ?? string.Empty,
            IncludeDeleted = includeDeleted ?? false
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
                    request.SystemName,
                    request.IncludeDeleted
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get registered systems by state
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="state"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("systems/registered/{state}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<RegisteredSystemResult>))]
    public async Task<IActionResult> GetRegisteredSystemByStateAsync(
        [FromServices] IRequestClient<GetSystemsByFilter> client,
        [FromRoute] RegisteredSystemState state,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
    {
        var request = new GetSystemsByFilterRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            RegisteredSystemState = state
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
                    request.RegisteredSystemState
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get all Rejected systems
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet("systems/rejected")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<RegisteredSystemRejectedResult>))]
    public async Task<IActionResult> GetRejectedSystemsAsync(
        [FromServices] IRequestClient<GetRegisteredSystemsRejected> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
    {
        var request = new GetRegisteredSystemsRejectedRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<RegisteredSystemRejectedResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get system by id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("systems/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisteredSystemResult))]
    public async Task<IActionResult> GetByIdAsync(
        [FromServices] IRequestClient<GetSystemById> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetSystemByIdRequest
        {
            Id = id
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<RegisteredSystemResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Modify event
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ModifyEventAsync(
        [FromServices] IRequestClient<ModifyEvent> client,
        [FromRoute] Guid id,
        [FromBody] EventModificationPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new EventModificationRequest
        {
            Id = id,
            IsDeleted = payload.IsDeleted
        };

        var logEventCode = LogEventCode.MODIFY_EVENT;
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("EventId", request.Id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(new
            {
                CorrelationId = RequestId,
                Id = id,
                ModifiedBy = GetUid(),
                request.IsDeleted
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Reject a system
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="requestPayload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of rejected system entity</returns>
    [HttpPut("systems/{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectSystemAsync(
        [FromServices] IRequestClient<RejectSystem> client,
        [FromRoute] Guid id,
        [FromBody] RejectSystemRequestPayload requestPayload,
        CancellationToken cancellationToken)
    {
        var request = new RejectSystemRequest
        {
            SystemId = id,
            Reason = requestPayload.Reason,
        };

        var logEventCode = LogEventCode.REJECT_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.SystemId), request.SystemId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SystemId,
                UserId = GetUid(),
                request.Reason
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Approve a system
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the approved system</returns>
    [HttpPut("systems/{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveSystemAsync(
        [FromServices] IRequestClient<ApproveSystem> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new ApproveSystemRequest
        {
            SystemId = id
        };

        var logEventCode = LogEventCode.APPROVE_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.SystemId), request.SystemId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SystemId,
                UserId = GetUid()
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Archive a system
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the archived system</returns>
    [HttpPut("systems/{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ArchiveSystemAsync(
        [FromServices] IRequestClient<ArchiveSystem> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new ArchiveSystemRequest
        {
            SystemId = id
        };

        var logEventCode = LogEventCode.ARCHIVE_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.SystemId), request.SystemId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SystemId,
                UserId = GetUid()
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Restore a system
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the restored system</returns>
    [HttpPut("systems/{id}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreSystemAsync(
        [FromServices] IRequestClient<RestoreSystem> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new RestoreSystemRequest
        {
            SystemId = id
        };

        var logEventCode = LogEventCode.RESTORE_SYSTEM;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.SystemId), request.SystemId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(new
            {
                CorrelationId = RequestId,
                request.SystemId,
                UserId = GetUid()
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Send notification to a user via users' selected channels or fallback to default(SMTP) channel.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether or not the system managed to queue the notification for sending</returns>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendAsync(
        [FromServices] IRequestClient<SendNotification> client,
        [FromBody] SendNotificationRequestInput input,
        CancellationToken cancellationToken)
    {
        var request = new SendNotificationRequest
        {
            UserId = input.UserId,
            EId = input.EId,
            Uid = input.Uid ?? string.Empty,
            EventCode = input.EventCode,
            Translations = input.Translations,
            UidType = input.UidType,
            SystemName = GetSystemName()
        };

        var logEventCode = LogEventCode.SEND_NOTIFICATION;
        var eventPayload = BeginAuditLog(logEventCode, request, request.EId.ToString(),
            (nameof(request.UserId), request.UserId ?? Guid.Empty),
            (AuditLoggingKeys.TargetUid, request.Uid),
            (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
            ("TargetEidentityId", request.EId),
            (nameof(request.EventCode), request.EventCode));

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
                request.Translations,
                request.UidType
            }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
