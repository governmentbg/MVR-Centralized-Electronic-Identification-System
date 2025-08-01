using System.Net;
using System.Net.Mime;
using eID.PDEAU.API.Public.Admin.Authorization;
using eID.PDEAU.API.Public.Admin.Requests;
using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.FilesManager;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PDEAU.API.Public.Admin.Controllers;

[RoleAuthorization(UserRoles.ApplicationAdministrator + UserRoles.Delimiter + UserRoles.ExternalSystemAdministrator)]
public partial class ProvidersController : BaseV1Controller
{
    private readonly FilesManager _filesManager;

    public ProvidersController(
        IConfiguration configuration,
        ILogger<ProvidersController> logger,
        AuditLogger auditLogger,
        FilesManager filesManager) : base(configuration, logger, auditLogger)
    {
        _filesManager = filesManager ?? throw new ArgumentNullException(nameof(filesManager));
    }

    /// <summary>
    /// Get provider by id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdministratorRegisteredProviderResult))]
    public async Task<IActionResult> GetProviderByIdAsync(
        [FromServices] IRequestClient<GetProviderById> client,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = ProviderId };

        var logEventCode = LogEventCode.GET_PROVIDER_BY_ID;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<AdministratorRegisteredProviderResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    IssuerUid = string.Empty,
                    IssuerUidType = IdentifierType.NotSpecified
                }, cancellationToken));

        return ResultWithAuditLogFromProviderResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("users", Name = nameof(RegisterUserAsync))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    public async Task<IActionResult> RegisterUserAsync(
        [FromServices] IRequestClient<RegisterUser> client,
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.REGISTER_USER;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (AuditLoggingKeys.TargetUid, request.Uid),
            (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
            (AuditLoggingKeys.TargetName, request.Name));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetProviderId(),
                    request.Uid,
                    request.UidType,
                    request.Name,
                    request.Email,
                    request.Phone
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("users/{id}", Name = nameof(UpdateUserAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserAsync(
        [FromServices] IRequestClient<UpdateUser> client,
        [FromRoute] Guid id,
        [FromBody] UpdateUserPayload payload,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.UPDATE_USER;

        var request = new UpdateUserRequest
        {
            Id = id
        };
        if (payload != null)
        {
            request.Uid = payload.Uid;
            request.UidType = payload.UidType;
            request.Name = payload.Name;
            request.Email = payload.Email;
            request.Phone = payload.Phone;
            request.IsAdministrator = payload.IsAdministrator;
        }
        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", id),
            (AuditLoggingKeys.TargetUid, request.Uid),
            (AuditLoggingKeys.TargetUidType, request.UidType.ToString()),
            (AuditLoggingKeys.TargetName, request.Name));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetProviderId(),
                    request.Id,
                    request.Uid,
                    request.UidType,
                    request.Name,
                    request.Email,
                    request.Phone,
                    request.IsAdministrator
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpDelete("users/{id}", Name = nameof(DeleteUserAsync))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteUserAsync(
        [FromServices] IRequestClient<DeleteUser> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.DELETE_USER;

        var request = new DeleteUserRequest
        {
            Id = id,
            ProviderId = ProviderId
        };

        var eventPayload = BeginAuditLog(logEventCode, request,
            ("UserId", id),
            (nameof(request.ProviderId), request.ProviderId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.Id,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get all users for provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("users", Name = nameof(GetUsersByFilterAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<UserResult>))]
    public async Task<IActionResult> GetUsersByFilterAsync(
        [FromServices] IRequestClient<GetUsersByFilter> client,
        [FromQuery] GetUsersByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_USERS_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<UserResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetProviderId(),
                    request.PageSize,
                    request.PageIndex,
                    request.Name,
                    request.Email,
                    request.IsAdministrator,
                    request.SortBy,
                    request.SortDirection
                }, cancellationToken));

        return ResultWithAuditLogFromUserResult(logEventCode, eventPayload, serviceResult, serviceResult.Result?.Data);
    }

    /// <summary>
    /// Get user details by Id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("users/{userId}", Name = nameof(GetUserDetailsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderResult))]
    public async Task<IActionResult> GetUserDetailsAsync(
        [FromServices] IRequestClient<GetUserDetails> client,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = userId };

        var logEventCode = LogEventCode.GET_USER_DETAILS;
        var eventPayload = BeginAuditLog(logEventCode,
            ("UserId", userId));

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<UserResult>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetProviderId(),
                    UserId = request.Id,
                }, cancellationToken));

        return ResultWithAuditLogFromUserResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Get provider services
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("services")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderServiceResult>))]
    public async Task<IActionResult> GetServicesByFilterAsync(
        [FromServices] IRequestClient<GetServicesByFilter> client,
        [FromQuery] GetServicesByFilterRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<ProviderServiceResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageIndex,
                    request.PageSize,
                    request.ServiceNumber,
                    request.Name,
                    request.Description,
                    request.FindServiceNumberAndName,
                    request.IncludeEmpowermentOnly,
                    request.IncludeDeleted,
                    ProviderId = GetProviderId(),
                    request.ProviderSectionId,
                    request.IncludeWithoutScope,
                    request.SortBy,
                    request.SortDirection,
                    request.IncludeApprovedOnly,
                    request.IncludeInactive
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Download the file for the provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="fileId"></param>
    [HttpGet("files/{fileId}", Name = nameof(GetProviderFileAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderFileAsync(
        [FromServices] IRequestClient<GetProviderFile> client,
        [FromRoute] Guid fileId,
        CancellationToken cancellationToken)
    {
        var request = new GetProviderFileRequest()
        {
            ProviderId = ProviderId,
            FileId = fileId
        };

        var logEventCode = LogEventCode.GET_PROVIDER_FILE;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            (nameof(request.FileId), request.FileId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<ProviderFileResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.FileId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IsPublic = false
                }, cancellationToken));

        if (serviceResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
        }

        if (serviceResult.Result is null || !FilesManager.FileExists(serviceResult.Result.FilePath))
        {
            Logger.LogError("Requested file {FileId} is missing.", fileId);

            eventPayload.Add("Reason", $"Requested file '{serviceResult.Result?.FilePath}' is missing.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.NotFound);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return NotFound();
        }

        try
        {
            var fileStream = _filesManager.Read(serviceResult.Result.FilePath);

            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload);

            return new FileStreamResult(fileStream, MediaTypeNames.Application.Pdf)
            {
                FileDownloadName = serviceResult.Result.FileName
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during reading file {FileId}.", fileId);

            eventPayload.Add("Reason", $"Error during reading file {fileId}.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.InternalServerError);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error during reading the file.");
        }
    }

    /// <summary>
    /// Get provider general information and offices
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("current/information")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderGeneralInformationAndOfficesResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderGeneralInformationAndOfficesByIdAsync(
        [FromServices] IRequestClient<GetProviderGeneralInformationAndOfficesById> client,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest
        {
            Id = ProviderId
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<ProviderGeneralInformationAndOfficesResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Update provider general information and offices
    /// </summary>
    /// <param name="client"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("current/information")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProviderGeneralInformationAndOfficesAsync(
        [FromServices] IRequestClient<UpdateProviderGeneralInformationAndOffices> client,
        [FromBody] UpdateProviderGeneralInformationAndOfficesPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new UpdateProviderGeneralInformationAndOfficesRequest
        {
            CorrelationId = RequestId,
            Id = ProviderId,
            GeneralInformation = payload.GeneralInformation,
            Offices = payload.Offices
        };

        var logEventCode = LogEventCode.UPDATE_PROVIDER_GENERAL_INFORMATION_AND_OFFICES;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(request, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Register how many time the service has been done from the provider.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("doneservice", Name = nameof(RegisterDoneServiceAsync))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterDoneServiceAsync(
        [FromServices] IRequestClient<RegisterDoneService> client,
        [FromBody] RegisterDoneServicePayload payload,
        CancellationToken cancellationToken)
    {
        var request = new RegisterDoneServiceRequest
        {
            ProviderId = ProviderId,
            ServiceName = payload.ServiceName,
            Count = payload.Count
        };

        var logEventCode = LogEventCode.REGISTER_DONE_SERVICE;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.ServiceName,
                    request.Count
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    private IActionResult ResultWithAuditLogFromUserResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<UserResult> serviceResult, UserResult? data)
        => Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }
            if (data is not null)
            {
                var currentPayload = new SortedDictionary<string, object>(eventPayload)
                {
                    [AuditLoggingKeys.TargetUid] = data.Uid,
                    [AuditLoggingKeys.TargetUidType] = data.UidType.ToString(),
                    [AuditLoggingKeys.TargetName] = data.Name,
                };
                AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });

    private IActionResult ResultWithAuditLogFromUserResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IPaginatedData<UserResult>> serviceResult, IEnumerable<UserResult>? data)
        => Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }
            if (data?.Any() == true)
            {
                foreach (var iUid in data.Select(r => r.Uid).Distinct())
                {
                    var provider = data.First(r => r.Uid == iUid);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        [AuditLoggingKeys.TargetUid] = provider.Uid,
                        [AuditLoggingKeys.TargetUidType] = provider.UidType.ToString(),
                        [AuditLoggingKeys.TargetName] = provider.Name,
                    };
                    AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });

    private IActionResult ResultWithAuditLogFromProviderResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<AdministratorRegisteredProviderResult> serviceResult, AdministratorRegisteredProviderResult? data)
        => Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }
            if (data is not null)
            {
                var currentPayload = new SortedDictionary<string, object>(eventPayload)
                {
                    [AuditLoggingKeys.TargetUid] = data.IssuerUid,
                    [AuditLoggingKeys.TargetUidType] = data.IssuerUidType.ToString(),
                    [AuditLoggingKeys.TargetName] = data.IssuerName,
                };
                AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });
}
