using System.Net;
using System.Net.Mime;
using eID.PDEAU.API.Public.Requests;
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

namespace eID.PDEAU.API.Public.Controllers;

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
    /// Register Provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{type}", Name = nameof(RegisterProviderAsync))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProviderAsync(
        [FromServices] IRequestClient<RegisterProvider> client,
        [FromRoute] ProviderType type,
        CancellationToken cancellationToken)
    {
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
        var request = JsonConvert.DeserializeObject<RegisterProviderRequest>(formData);
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
                    ExternalNumber = string.Empty,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    CreatedByAdministratorId = string.Empty,
                    CreatedByAdministratorName = string.Empty,
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

        // We need persisted record in order to use its id when we handle the uploaded files.
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
                    ProviderId = GetSystemId(),
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
                    ProviderId = GetSystemId(),
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
                    ProviderId = GetSystemId(),
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
    public async Task<IActionResult> GetUserDetailsAsync(
        [FromServices] IRequestClient<GetUserDetails> client,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_USER_DETAILS;
        var eventPayload = BeginAuditLog(logEventCode, null,
            ("UserId", userId));

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<UserResult>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = GetSystemId(),
                    UserId = userId,
                }, cancellationToken));

        return ResultWithAuditLogFromUserResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Initiate a process for promoting new Administrator for given Provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{providerId}/users/{userId}/promote-to-admin", Name = nameof(InitiateAdminPromotionAsync))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    public async Task<IActionResult> InitiateAdminPromotionAsync(
        [FromServices] IRequestClient<InitiateAdminPromotion> client,
        [FromRoute] InitiateAdminPromotionRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.INITIATE_ADMIN_PROMOTION;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }
        eventPayload[nameof(request.ProviderId)] = request.ProviderId;
        eventPayload[nameof(request.UserId)] = request.UserId;

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.ProviderId,
                    request.UserId,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Finishes the process of promotion of new Administrator by handling the callback url from confirmation email
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("administrator-promotions/{administratorPromotionId}", Name = nameof(ConfirmAdminPromotionCallbackAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmAdminPromotionCallbackAsync(
        [FromServices] IRequestClient<ConfirmAdminPromotion> client,
        [FromRoute] ConfirmAdminPromotionRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.CONFIRM_ADMIN_PROMOTION;
        var eventPayload = BeginAuditLog(logEventCode, request);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }
        eventPayload[nameof(request.AdministratorPromotionId)] = request.AdministratorPromotionId;

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<bool>>(
                new
                {
                    CorrelationId = RequestId,
                    request.AdministratorPromotionId,
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
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
                    request.ProviderId,
                    request.ProviderSectionId,
                    request.IncludeWithoutScope,
                    request.SortBy,
                    request.SortDirection,
                    IncludeInactive = false,
                    IncludeApprovedOnly = true
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get provider by id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    [HttpGet("{id}")]
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

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<AdministratorRegisteredProviderResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType()
                }, cancellationToken));

        return ResultWithAuditLogFromProviderResult(logEventCode, eventPayload, serviceResult, serviceResult.Result);
    }

    /// <summary>
    /// Get all Providers, no filtering
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    [AllowAnonymous]
    [HttpGet("list", Name = nameof(GetAllProvidersAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderResult>))]
    public async Task<IActionResult> GetAllProvidersAsync(
        [FromServices] IRequestClient<GetAllProviders> client,
        [FromQuery] GetAllProvidersRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
                    client.GetResponse<ServiceResult<IPaginatedData<ProviderInfoResult>>>(
                        new
                        {
                            CorrelationId = RequestId,
                            request.PageSize,
                            request.PageIndex,
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
        _ = Guid.TryParse(GetSystemId(), out var providerId);
        var request = new GetProviderFileRequest()
        {
            ProviderId = providerId,
            FileId = fileId
        };

        var logEventCode = LogEventCode.GET_PROVIDER_FILE;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(request.ProviderId), request.ProviderId),
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
                    IsPublic = true,
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
    /// Get all Providers
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    [HttpGet(Name = nameof(GetProvidersByFilterAsync))]
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
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    request.Number
                }, cancellationToken));

        return ResultWithAuditLogFromProviderResult(logEventCode, eventPayload, serviceResult, serviceResult.Result?.Data);
    }

    /// <summary>
    /// Enables updates of provider information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id">Provider id</param>
    /// <param name="cancellationToken"></param>
    [HttpPut("{id}", Name = nameof(UpdateProviderAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProviderAsync(
        [FromServices] IRequestClient<UpdateProvider> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.UPDATE_PROVIDER;
        var eventPayload = BeginAuditLog(logEventCode, Request.Form?["data"],
            ("ProviderId", id));

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
        if (!formData.Any())
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("Form", "Form data is required.");

            eventPayload.Add("Reason", "Form data is required.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return ValidationProblem(msd);
        }

        var request = JsonConvert.DeserializeObject<UpdateProviderRequest>(formData);
        if (request is null)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("data", "Unable to parse data.");

            eventPayload.Add("Reason", "Unable to parse data.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);

            return ValidationProblem(msd);
        }
        request.ProviderId = id;

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        // Check files
        var newFiles = Request.Form.Files;
        // Skip validation if no files
        if (newFiles.Any())
        {
            var uploadedFiles = _filesManager.GetFolderFileNameList(request.ProviderId.ToString());

            var filesValidator = new UpdateProviderFormFilesValidator(uploadedFiles);
            var filesValidationResult = filesValidator.Validate(newFiles);
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
        }

        // Upload files
        var fileData = new List<FileData>();
        var filesUploadedSuccessfully = true;
        foreach (var file in newFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);

            var uploadedFilePath = await _filesManager.SaveAsync(request.ProviderId.ToString(), fileName, fileExtension, file.OpenReadStream());
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                Logger.LogError("Failed saving file: {FileName} for ProviderId: {ProviderId}", fileName, request.ProviderId);
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
            client.GetResponse<ServiceResult>(
                new
                {
                    request.ProviderId,
                    CorrelationId = RequestId,
                    IssuerUid = GetUid(),
                    IssuerUidType = GetUidType(),
                    IssuerName = GetUserFullName(),
                    request.Comment,
                    FilesInformation = filesInformation
                }, cancellationToken));


        if (serviceResult.StatusCode >= HttpStatusCode.BadRequest && fileData.Count > 0)
        {
            Logger.LogError("Update provider was not successful. {FileCount} uploaded files will be deleted", fileData.Count);
            _filesManager.RemoveFiles(fileData.Select(d => d.FullFilePath).ToArray());
        }

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviderStatusHistoryAsync(
        [FromServices] IRequestClient<GetProviderStatusHistory> client,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = id };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ProviderStatusHistoryResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get providers information
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("info", Name = nameof(GetProvidersInfoAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<ProviderInfoResult>))]
    public async Task<IActionResult> GetProvidersInfoAsync(
        [FromServices] IRequestClient<GetProvidersInfo> client,
        [FromQuery] GetProviderInfoRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<ProviderInfoResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageIndex,
                    request.PageSize,
                    request.Name,
                    request.IdentificationNumber,
                    request.Bulstat,
                    request.SortBy,
                    request.SortDirection,
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get provider's offices
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{providerId}/offices")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IProviderOffice>))]
    public async Task<IActionResult> GetProviderOfficesAsync(
        [FromServices] IRequestClient<GetProviderOffices> client,
        [FromRoute] Guid providerId,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = providerId };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<IProviderOffice>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get provider's services
    /// </summary>
    /// <param name="client"></param>
    /// <param name="providerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{providerId}/services")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProviderServiceInfoResult>))]
    public async Task<IActionResult> GetProviderServicesAsync(
        [FromServices] IRequestClient<GetProviderServices> client,
        [FromRoute] Guid providerId,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdRequest { Id = providerId };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ProviderServiceInfoResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        return Result(serviceResult);
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
}
