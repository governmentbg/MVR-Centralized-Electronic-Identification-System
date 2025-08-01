using System.Net;
using System.Net.Mime;
using eID.PDEAU.API.Authorization;
using eID.PDEAU.API.Requests;
using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.FilesManager;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace eID.PDEAU.API.Controllers;

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
    /// Get all Providers
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    [HttpGet(Name = nameof(GetProvidersByFilterAsync))]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderResult>))]
    public async Task<IActionResult> GetProvidersByFilterAsync(
        [FromServices] IRequestClient<GetProvidersByFilter> client,
        [FromQuery] GetProvidersByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PROVIDERS_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }
        
        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<ProviderResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    request.Status,
                    request.ProviderName,
                    request.SortBy,
                    request.SortDirection,
                    IssuerUid = string.Empty, // In order to fetch all providers regardless of the issuer
                    IssuerUidType = IdentifierType.NotSpecified,
                    request.Number,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLogFromProviderResult(logEventCode, eventPayload, serviceResult, serviceResult.Result?.Data);
    }

    /// <summary>
    /// Get list all providers
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    [HttpGet("list")]
    [RoleAuthorization(allowM2M: true)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderListResult>))]
    public async Task<IActionResult> GetProvidersListByFilterAsync(
        [FromServices] IRequestClient<GetProvidersListByFilter> client,
        [FromQuery] GetProvidersByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PROVIDERS_LIST_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<ProviderListResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex,
                    request.Status,
                    request.ProviderName,
                    request.SortBy,
                    request.SortDirection,
                    IssuerUid = string.Empty,
                    IssuerUidType = Contracts.Enums.IdentifierType.NotSpecified
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Approve provider with <paramref name="providerId"/>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{providerId}/approve")]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveProviderAsync(
        [FromServices] IRequestClient<ApproveProvider> client,
        [FromRoute] Guid providerId,
        [FromBody] ProviderPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new ApproveProviderRequest
        {
            ProviderId = providerId,
            Comment = payload.Comment
        };

        var logEventCode = LogEventCode.APPROVE_PROVIDER;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            (nameof(request.ProviderId), request.ProviderId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.Comment,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Return provider <paramref name="providerId"/> for correction
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{providerId}/return")]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnProviderAsync(
        [FromServices] IRequestClient<ReturnProviderForCorrection> client,
        [FromRoute] Guid providerId,
        [FromBody] ProviderPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new ReturnProviderRequest
        {
            ProviderId = providerId,
            Comment = payload.Comment
        };

        var logEventCode = LogEventCode.REGISTER_PROVIDER;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            (nameof(request.ProviderId), request.ProviderId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.Comment,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get provider by id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdministratorRegisteredProviderResult))]
    public async Task<IActionResult> GetProviderByIdAsync(
        [FromServices] IRequestClient<GetProviderById> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.GET_PROVIDER_BY_ID;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            ("ProviderId", id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<AdministratorRegisteredProviderResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    IssuerUid = string.Empty,
                    IssuerUidType = IdentifierType.NotSpecified,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLogFromProviderResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Get providers of electronic administrative services
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("details/available")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProviderDetailsResult>))]
    public async Task<IActionResult> GetAvailableProviderDetailsByFilterAsync(
        [FromServices] IRequestClient<GetAvailableProviderDetailsByFilter> client,
        [FromQuery] GetAvailableProviderDetailsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_AVAILABLE_PROVIDER_DETAILS_BY_FILTER;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ProviderDetailsResult>>>(
                new
                {
                    CorrelationId = RequestId
                }, cancellationToken));

        return ResultWithAuditLogFromProviderDetailResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Register Provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{type}", Name = nameof(RegisterProviderAsync))]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProviderAsync(
        [FromServices] IRequestClient<RegisterProvider> client,
        [FromRoute] ProviderType type,
        CancellationToken cancellationToken)
    {
        if (IsUserInPLSRole() && type != ProviderType.PrivateLawSubject)
        {
            Logger.LogWarning("Role '{Role}' allows only of ProviderType '{ProviderType}' to be registered", UserRoles.PLSAdministrator, ProviderType.PrivateLawSubject);
            return Forbid();
        }

        var logEventCode = LogEventCode.REGISTER_PROVIDER;
        var eventPayload = BeginAuditLog(logEventCode, Request.Form?["data"]);

        if (Request.Form is null)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("Form", "Form is required.");

            eventPayload.Add("Reason", "Form is required.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return ValidationProblem(msd);
        }
        var formData = Request.Form["data"];
        var request = JsonConvert.DeserializeObject<AdministratorRegisterProviderRequest>(formData);
        if (request is null)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("data", "Unable to parse data.");

            eventPayload.Add("Reason", "Unable to parse data.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return ValidationProblem(msd);
        }
        request.Type = type;

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var files = Request.Form.Files;
        var filesValidator = new RegisterProviderFormFilesValidator();
        var filesValidationResult = filesValidator.Validate(files);
        if (!filesValidationResult.IsValid)
        {
            var msd = new ModelStateDictionary();
            filesValidationResult?.Errors?.ForEach(error =>
            {
                msd.AddModelError(error.PropertyName, error.ErrorMessage);
            });

            eventPayload.Add("Reason", string.Join(",", filesValidationResult.GetValidationErrorList()));
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return ValidationProblem(msd);
        }

        var providerId = Guid.NewGuid();

        // Upload files
        var fileData = new List<FileData>();
        var filesUploadedSuccessfully = true;
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);

            var uploadedFilePath = await _filesManager.SaveAsync(providerId.ToString(), fileName, fileExtension, file.OpenReadStream());
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                Logger.LogError("Failed saving file: {FileName} for ProviderId: {ProviderId}", fileName, providerId);
                filesUploadedSuccessfully = false;
                break;
            }
            
            fileData.Add(new FileData { FullFilePath = uploadedFilePath, FileName = file.FileName });
        }

        if (!filesUploadedSuccessfully)
        {
            Logger.LogError("Files upload was not successful. {FileCount} uploaded files will be deleted", fileData.Count);
            _filesManager.RemoveFiles(fileData.Select(d => d.FullFilePath).ToArray());

            eventPayload["Reason"] = "Files upload was not successful";
            eventPayload["ResponseStatusCode"] = HttpStatusCode.Conflict;
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return Conflict("File upload problem");
        }

        FilesInformation filesInformation = new FilesInformationDTO
        {
            UploaderUid = GetUid(),
            UploaderUidType = GetUidType(),
            UploaderName = GetUserFullName(),
            Files = fileData
        };

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    ProviderId = providerId,
                    CorrelationId = RequestId,
                    request.ExternalNumber,
                    IssuerUid = request.Issuer.Uid,
                    IssuerUidType = request.Issuer.UidType,
                    IssuerName = request.Issuer.Name,
                    CreatedByAdministratorId = GetUserId(),
                    CreatedByAdministratorName = GetUserFullName(),
                    request.Name,
                    request.Type,
                    request.Bulstat,
                    request.Headquarters,
                    request.Address,
                    request.Email,
                    request.Phone,
                    Administrator = request.Administrator as RegisterProviderUser,
                    FilesInformation = filesInformation
                }, cancellationToken, RequestTimeout.After(m: 2)));

        if (serviceResult.StatusCode >= HttpStatusCode.BadRequest)
        {
            Logger.LogError("Register provider was not successful. {FileCount} uploaded files will be deleted", fileData.Count);
            _filesManager.RemoveFiles(fileData.Select(d => d.FullFilePath).ToArray());
        }
                
        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    private class FilesInformationDTO : FilesInformation
    {
        public string UploaderUid { get; set; } = string.Empty;
        public IdentifierType UploaderUidType { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public IEnumerable<FileData> Files { get; set; } = new List<FileData>();
    }

    /// <summary>
    /// Download the file for the provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="fileId"></param>
    [HttpGet("{providerId}/files/{fileId}", Name = nameof(GetProviderFileAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderFileAsync(
        [FromServices] IRequestClient<GetProviderFile> client,
        [FromRoute] Guid providerId,
        [FromRoute] Guid fileId,
        CancellationToken cancellationToken)
    {
        var request = new GetProviderFileRequest()
        {
            ProviderId = providerId,
            FileId = fileId
        };

        var logEventCode = LogEventCode.GET_PROVIDER_FILE;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.ProviderId), request.ProviderId),
            (nameof(request.FileId), request.FileId)
        );

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
                    IssuerUid = string.Empty,
                    IssuerUidType = Contracts.Enums.IdentifierType.NotSpecified,
                    IsPublic = false
                }, cancellationToken));

        if (serviceResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
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
    /// Deny provider with <paramref name="providerId"/>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{providerId}/deny")]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DenyProviderAsync(
        [FromServices] IRequestClient<DenyProvider> client,
        [FromRoute] Guid providerId,
        [FromBody] ProviderPayload payload,
        CancellationToken cancellationToken)
    {
        var request = new CancelProviderRequest
        {
            ProviderId = providerId,
            Comment = payload.Comment
        };

        var logEventCode = LogEventCode.DENY_PROVIDER;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            (nameof(request.ProviderId), request.ProviderId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.Comment,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get provider status history by <paramref name="id"/>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}/statushistory")]
    [AllowForbiddenRoles(UserRoles.PLSAdministrator)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviderStatusHistoryAsync(
        [FromServices] IRequestClient<GetProviderStatusHistory> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };

        var logEventCode = LogEventCode.GET_PROVIDER_STATUS_HISTORY;
        var eventPayload = BeginAuditLog(logEventCode, request, 
            ("ProviderId", id));

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var isPLSRole = IsUserInPLSRole();
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ProviderStatusHistoryResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = request.Id,
                    IsPLSRole = isPLSRole
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get user for provider by Uid
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id}/users", Name = nameof(GetUserByUidAsync))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserByUidResult))]
    public async Task<IActionResult> GetUserByUidAsync(
        [FromServices] IRequestClient<GetUserByUid> client,
        [FromRoute] Guid id,
        [FromQuery] GetUserByUidQuery query,
        CancellationToken cancellationToken)
    {
        var request = new GetUserByUidRequest
        {
            ProviderId = id,
            Uid = query.Uid,
            UidType = query.UidType
        };

        var logEventCode = LogEventCode.GET_USER_BY_UID;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<UserByUidResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.Uid,
                    request.UidType
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Register user by MoI administrator
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("users", Name = nameof(AdministratorRegisterUserAsync))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    public async Task<IActionResult> AdministratorRegisterUserAsync(
        [FromServices] IRequestClient<AdministratorRegisterUser> client,
        [FromBody] AdministratorRegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.REGISTER_USER;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.ProviderId), request.ProviderId),
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
                    AdministratorUid = GetUid(),
                    AdministratorUidType = GetUidType(),
                    AdministratorFullName = GetUserFullName(),
                    request.ProviderId,
                    request.Uid,
                    request.UidType,
                    request.Name,
                    request.Email,
                    request.Phone,
                    request.IsAdministrator,
                    request.Comment
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Update user by MoI administrator
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("{id}/users/{userId}", Name = nameof(AdministratorUpdateUserAsync))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AdministratorUpdateUserAsync(
        [FromServices] IRequestClient<AdministratorUpdateUser> client,
        [FromRoute] Guid id,
        [FromRoute] Guid userId,
        [FromBody] AdministratorUpdateUserPayload payload,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.UPDATE_USER;

        var request = new AdministratorUpdateUserRequest
        {
            ProviderId = id,
            Id = userId,
        };

        if (payload != null)
        {
            request.IsAdministrator = payload.IsAdministrator;
            request.Comment = payload.Comment;
        }

        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.ProviderId), request.ProviderId),
            ("UserId", id));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    AdministratorUid = GetUid(),
                    AdministratorUidType = GetUidType(),
                    AdministratorFullName = GetUserFullName(),
                    request.ProviderId,
                    request.Id,
                    request.IsAdministrator,
                    request.Comment
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get User's Administrator Actions
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    [HttpGet("{id}/users/{userId}/administratorActions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AdministratorActionResult>))]
    public async Task<IActionResult> GetUserAdministratorActionsAsync(
        [FromServices] IRequestClient<GetUserAdministratorActions> client,
        [FromRoute] Guid id,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var request = new GetUserAdministratorActionsRequest 
        { 
            ProviderId = id,
            UserId = userId,
        };

        var logEventCode = LogEventCode.GET_USER_ADMINISTRATOR_ACTIONS;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.ProviderId), request.ProviderId),
            (nameof(request.UserId), request.UserId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<AdministratorActionResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.UserId,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

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

    private IActionResult ResultWithAuditLogFromProviderResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IPaginatedData<ProviderResult>> serviceResult, IEnumerable<ProviderResult>? data)
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
                foreach (var iUid in data.Select(r => r.IssuerUid).Distinct())
                {
                    var provider = data.First(r => r.IssuerUid == iUid);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        [AuditLoggingKeys.TargetUid] = provider.IssuerUid,
                        [AuditLoggingKeys.TargetUidType] = provider.IssuerUidType.ToString(),
                        [AuditLoggingKeys.TargetName] = provider.IssuerName
                    };
                    AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });

    private IActionResult ResultWithAuditLogFromProviderDetailResult(LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, ServiceResult<IEnumerable<ProviderDetailsResult>> serviceResult, IEnumerable<ProviderDetailsResult>? data)
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
                foreach (var uic in data.Where(d => !d.SyncedFromOnlineRegistry).Select(r => r.UIC).Distinct())
                {
                    var provider = data.First(r => r.UIC == uic);
                    var currentPayload = new SortedDictionary<string, object>(eventPayload)
                    {
                        ["ProviderId"] = provider.Id
                    };
                    AddAuditLog(logEventCode, suffix: suffix, payload: currentPayload);
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });
}
