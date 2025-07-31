using System.Net;
using System.Net.Http.Headers;
using eID.MIS.API.Responses;
using eID.MIS.Contracts;
using eID.MIS.Contracts.Enums;
using eID.MIS.Contracts.Requests;
using eID.MIS.Contracts.Results;
using eID.MIS.Contracts.SEV.Commands;
using eID.MIS.Contracts.SEV.External;
using eID.MIS.Contracts.SEV.Results;
using eID.MIS.Contracts.SEV.Validators;
using eID.PJS.AuditLogging;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace eID.MIS.API.Controllers;

[AllowAnonymous]
//[RoleAuthorization(UserRoles.AllAdmins, allowM2M: true)]
[Route("[controller]/api/v{version:apiVersion}/edelivery")]
public class MissevController : BaseV1Controller
{
    public MissevController(IConfiguration configuration, ILogger<MissevController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    [HttpPost("create-passive-individual-profile", Name = nameof(CreatePassiveIndividualProfileAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatePassiveIndividualProfileResult))]
    public async Task<IActionResult> CreatePassiveIndividualProfileAsync(
        [FromServices] IRequestClient<CreatePassiveIndividualProfile> client,
        [FromQuery] Guid eIdentityId,
        [FromBody] CreatePassiveIndividualProfileRequest payload,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.CREATE_PASSIVE_INDIVIDUAL_PROFILE;
        var command = new CreatePassiveIndividualProfileCommand
        {
            CorrelationId = RequestId,
            EIdentityId = eIdentityId,
            Request = payload,
            SystemName = GetSystemName()
        };
        var eventPayload = BeginAuditLog(logEventCode, command, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId),
            ("SystemName", command.SystemName));
        if (!command.IsValid())
        {
            return BadRequestWithAuditLog(command, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<CreatePassiveIndividualProfileResult>>(command, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    internal class CreatePassiveIndividualProfileCommand : CreatePassiveIndividualProfile, IValidatableRequest
    {
        public Guid EIdentityId { get; set; }

        public CreatePassiveIndividualProfileRequest Request { get; set; }

        public string SystemName { get; set; }

        public Guid CorrelationId { get; set; }

        public IValidator GetValidator() => new CreatePassiveIndividualProfileValidator();
    }


    [HttpGet("search-profile", Name = nameof(SearchUserProfileAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchUserProfileResult))]
    public async Task<IActionResult> SearchUserProfileAsync(
        [FromServices] IRequestClient<SearchUserProfile> client,
        [FromQuery] SearchUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEARCH_PROFILE;
        var command = new SearchProfileCommand
        {
            CorrelationId = RequestId,
            EIdentityId = query.EIdentityId,
            Identifier = query.Identifier,
            TargetGroupId = query.TargetGroupId
        };
        var eventPayload = BeginAuditLog(logEventCode, command, targetUserId: command.EIdentityId.ToString(),
            ("EIdentityId", command.EIdentityId));
        if (!command.IsValid())
        {
            return BadRequestWithAuditLog(command, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<SearchUserProfileResult>>(command, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    internal class SearchProfileCommand : SearchUserProfile, IValidatableRequest
    {
        public Guid EIdentityId { get; set; }
        public string Identifier { get; set; }
        public string TargetGroupId { get; set; }

        public Guid CorrelationId { get; set; }

        public IValidator GetValidator() => new SearchUserProfileValidator();
    }

    [HttpGet("get-profile/{profileId}", Name = nameof(GetUserProfileAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProfileResult))]
    public async Task<IActionResult> GetUserProfileAsync(
        [FromServices] IRequestClient<GetUserProfile> client,
        [FromRoute] string profileId,
        [FromQuery] Guid eIdentityId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_PROFILE;
        var command = new GetUserProfileCommand
        {
            CorrelationId = RequestId,
            ProfileId = profileId,
            EIdentityId = eIdentityId
        };
        var eventPayload = BeginAuditLog(logEventCode, command, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId));

        if (!command.IsValid())
        {
            return BadRequestWithAuditLog(command, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<GetProfileResult>>(command, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    internal class GetUserProfileCommand : GetUserProfile, IValidatableRequest
    {
        public Guid EIdentityId { get; set; }
        public string ProfileId { get; set; }

        public Guid CorrelationId { get; set; }

        public IValidator GetValidator() => new GetUserProfileValidator();
    }

    [HttpPost("send-message-on-behalf", Name = nameof(SendMessageOnBehalfAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SendMessageResult))]
    public async Task<IActionResult> SendMessageOnBehalfAsync(
        [FromServices] IRequestClient<SendMessageOnBehalf> client,
        [FromQuery] Guid eIdentityId,
        [FromBody] SendMessageOnBehalfRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_MESSAGE_ON_BEHALF;
        var command = new SendMessageOnBehalfCommand
        {
            CorrelationId = RequestId,
            Request = request,
            EIdentityId = eIdentityId,
            SystemName = GetSystemName()
        };
        var eventPayload = BeginAuditLog(logEventCode, command, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId),
            ("SystemName", command.SystemName));

        if (!command.IsValid())
        {
            return BadRequestWithAuditLog(command, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<SendMessageResult>>(command, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    internal class SendMessageOnBehalfCommand : SendMessageOnBehalf, IValidatableRequest
    {
        public Guid EIdentityId { get; set; }
        public SendMessageOnBehalfRequest Request { get; set; }
        public string SystemName { get; set; }

        public Guid CorrelationId { get; set; }

        public IValidator GetValidator() => new SendMessageOnBehalfValidator();
    }


    [HttpPost("send-message", Name = nameof(SendMessageAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SendMessageResult))]
    public async Task<IActionResult> SendMessageAsync(
        [FromServices] IRequestClient<SendMessage> client,
        [FromQuery] Guid eIdentityId,
        [FromQuery] string uid,
        [FromQuery] IdentifierType uidType,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.SEND_MESSAGE;
        var command = new SendMessageCommand
        {
            CorrelationId = RequestId,
            Request = request,
            EIdentityId = eIdentityId,
            SystemName = GetSystemName()
        };
        var eventPayload = BeginAuditLog(logEventCode, command, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId),
            ("SystemName", command.SystemName),
            ( AuditLoggingKeys.TargetUid, uid ),
            ( AuditLoggingKeys.TargetUidType, uidType.ToString() ));
        if (!command.IsValid())
        {
            return BadRequestWithAuditLog(command, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<SendMessageResult>>(command, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    internal class SendMessageCommand : SendMessage, IValidatableRequest
    {
        public Guid EIdentityId { get; set; }
        public SendMessageRequest Request { get; set; }
        public string SystemName { get; set; }

        public Guid CorrelationId { get; set; }

        public IValidator GetValidator() => new SendMessageValidator();
    }

    [HttpPost("upload/blobs", Name = nameof(UploadBlobAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadFileResponse))]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadBlobAsync(
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] Guid eIdentityId,
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.UPLOAD_BLOB;
        var eventPayload = BeginAuditLog(logEventCode, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId));
        if (files is null || !files.Any() || files.Count > 1)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(files), $"{nameof(files)} is required and must contain exactly one file.");
            eventPayload.Add("Reason", $"{nameof(files)} is required and must contain exactly one file.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }
        var content = new MultipartFormDataContent();
        foreach (var file in files)
        {
            try
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, file.Name, file.FileName);
            }
            catch (Exception ex)
            {
                continue;
            }
        }

        try
        {
            var proxyRequest = new HttpRequestMessage(HttpMethod.Post, "eDelivery/upload/blobs")
            {
                Content = content
            };
            var client = httpClientFactory.CreateClient("Integrations");
            SetRequestIdDefaultHeader(client);
            var response = await client.SendAsync(proxyRequest, cancellationToken);

            response.EnsureSuccessStatusCode();
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.OK);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload);
            return Result(
                new ServiceResult<UploadFileResponse>
                {
                    Result = JsonConvert.DeserializeObject<UploadFileResponse>(
                        await response.Content.ReadAsStringAsync(cancellationToken)
                    ),
                    StatusCode = HttpStatusCode.OK
                });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception encountered during iteration of form files.");
            eventPayload.Add("Reason", $"Exception encountered during iteration of form files.");
            return ResultWithAuditLog(new ServiceResult<UploadFileResponse> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" }, logEventCode, eventPayload);
        }
    }

    [HttpPost("upload/obo/blobs", Name = nameof(UploadBlobOnBehalfOfAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadFileResponse))]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadBlobOnBehalfOfAsync(
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] Guid eIdentityId,
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.UPLOAD_BLOB_ON_BEHALF_OF;
        var eventPayload = BeginAuditLog(logEventCode, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId));
        if (files is null || !files.Any() || files.Count > 1)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(files), $"{nameof(files)} is required and must contain exactly one file.");
            eventPayload.Add("Reason", $"{nameof(files)} is required and must contain exactly one file.");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }
        var content = new MultipartFormDataContent();
        foreach (var file in files)
        {
            try
            {
                content.Add(new StreamContent(file.OpenReadStream()));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception encountered during iteration of form files.");
                continue;
            }
        }

        try
        {
            var proxyRequest = new HttpRequestMessage(HttpMethod.Post, "eDelivery/upload/obo/blobs")
            {
                Content = content
            };
            var client = httpClientFactory.CreateClient("Integrations");
            SetRequestIdDefaultHeader(client);
            var response = await client.SendAsync(proxyRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            eventPayload.Add("ResponseStatusCode", HttpStatusCode.OK);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload);

            return Result(
                new ServiceResult<UploadFileResponse>
                {
                    Result = JsonConvert.DeserializeObject<UploadFileResponse>(
                        await response.Content.ReadAsStringAsync(cancellationToken)
                    ),
                    StatusCode = HttpStatusCode.OK
                });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception encountered during iteration of form files.");
            eventPayload.Add("Reason", $"Exception encountered during iteration of form files.");
            return ResultWithAuditLog(new ServiceResult<UploadFileResponse> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" }, logEventCode, eventPayload);
        }
    }

    [HttpGet("deliveries", Name = nameof(GetDeliveriesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DeliveryRequestResult>))]
    public async Task<IActionResult> GetDeliveriesAsync(
        [FromServices] IRequestClient<GetDeliveries> client,
        [FromQuery] Guid eIdentityId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_DELIVERIES;
        var eventPayload = BeginAuditLog(logEventCode, targetUserId: eIdentityId.ToString(),
            ("EIdentityId", eIdentityId));
        if (Guid.Empty == eIdentityId)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError(nameof(eIdentityId), $"Invalid parameter {nameof(eIdentityId)}");
            eventPayload.Add("Reason", $"Invalid parameter {nameof(eIdentityId)}");
            eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<DeliveryRequestResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    EIdentityId = eIdentityId
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
