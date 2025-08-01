using System.Net;
using Asp.Versioning;
using eID.PDEAU.API.Public.Requests;
using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PJS.AuditLogging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace eID.PDEAU.API.Public.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
[ProducesResponseType(StatusCodes.Status504GatewayTimeout, Type = typeof(ProblemDetails))]
[SetRequestId]
public class BaseV1Controller : ControllerBase
{
    internal ILogger Logger { get; private set; }
    private readonly AuditLogger _auditLogger;
    private readonly IConfiguration _configuration;

    public BaseV1Controller(IConfiguration configuration, ILogger logger, AuditLogger auditLogger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    }

    public Guid RequestId { get; set; }

    protected void SetRequestIdDefaultHeader(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.RequestId, RequestId.ToString());
    }

    protected IActionResult Result<T>(ServiceResult<T> data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            return StatusCode((int)data.StatusCode, data.Result);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected IActionResult Result(ServiceResult data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            return StatusCode((int)data.StatusCode);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected IActionResult BadRequest(IValidatableRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var validationResult = request.GetValidationResult();

        var msd = new ModelStateDictionary();

        validationResult?.Errors?.ForEach(error =>
        {
            msd.AddModelError(error.PropertyName, error.ErrorMessage);
        });

        return ValidationProblem(msd);
    }

    /// <summary>
    /// Preset "traceId" with our <see cref="RequestId"/>
    /// </summary>
    [NonAction]
    public override ObjectResult Problem(
            string? detail = null,
            string? instance = null,
            int? statusCode = null,
            string? title = null,
            string? type = null)
    {
        var result = base.Problem(detail, instance, statusCode, title, type);

        var problemDetails = (ProblemDetails)result.Value!;

        problemDetails.Extensions["traceId"] = RequestId;

        return result;
    }

    /// <summary>
    /// Preset "traceId" with our <see cref="RequestId"/>
    /// </summary>
    [NonAction]
    public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        var result = (ObjectResult)base.ValidationProblem(modelStateDictionary);

        var problemDetails = (ValidationProblemDetails)result.Value!;

        problemDetails.Extensions["traceId"] = RequestId;

        return result;
    }

    protected async Task<ServiceResult<TResult>> GetResponseAsync<TResult>(Func<Task<Response<ServiceResult<TResult>>>> client)
    {
        try
        {
            var result = await client();

            return result.Message;
        }
        catch (RequestTimeoutException ex)
        {
            Logger.LogError(ex, "Application call timeout. RequestId: {RequestId}", RequestId);
            return new ServiceResult<TResult> { StatusCode = HttpStatusCode.GatewayTimeout, Error = "Application timeout" };
        }
    }

    protected async Task<ServiceResult> GetResponseAsync(Func<Task<Response<ServiceResult>>> client)
    {
        try
        {
            var result = await client();

            return result.Message;
        }
        catch (RequestTimeoutException ex)
        {
            Logger.LogError(ex, "Application call timeout. RequestId: {RequestId}", RequestId);
            return new ServiceResult { StatusCode = HttpStatusCode.GatewayTimeout, Error = "Application timeout" };
        }
    }

    protected void AddLog(LogEventCode logEvent, Dictionary<string, object>? customFields, LogLevel logLevel = LogLevel.Information)
    {
        var userId = GetUserId();

        var message = "{Id} {EventCode} {UserId}";
        var parameters = new List<object> {
            Guid.NewGuid(),
            logEvent,
            userId
        };

        if (customFields?.Any() ?? false)
        {
            message = $"{message} {string.Join(',', customFields.Keys.Select(k => string.Concat("{", k, "}")))}";
            parameters.AddRange(customFields.Values);
        }

        Logger.Log(logLevel, message, parameters.ToArray());
    }
    protected string GetUserId() =>
        HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.CitizenProfileId)?.Value ?? string.Empty;

    protected string GetUid() =>
        HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.CitizenIdentifier)?.Value ?? string.Empty;

    protected IdentifierType GetUidType()
    {
        var str = HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.CitizenIdentifierType)?.Value;
        if (!Enum.TryParse<IdentifierType>(str, true, out var result))
        { return IdentifierType.NotSpecified; }
        return result;
    }

    protected string GetUserFullName()
    {
        var givenName = HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.GivenNameCyrillic)?.Value;
        var middleName = HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.MiddleNameCyrillic)?.Value;
        var familyName = HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.FamilyNameCyrillic)?.Value;
        return string.Join(" ", new[] { givenName, middleName, familyName }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    protected string? GetSystemId() =>
       HttpContext.User.Claims.FirstOrDefault(d => d.Type == Claims.SystemId)?.Value;

    protected void AddAuditLog(
        LogEventCode logEvent,
        LogEventLifecycle suffix,
        string? targetUserId = default,
        SortedDictionary<string, object>? payload = default)
    {
        if (_configuration.GetValue<bool>("SkipAuditLogging"))
        {
            return;
        }

        _auditLogger.LogEvent(new AuditLogEvent
        {
            CorrelationId = RequestId.ToString(),
            RequesterSystemId = HttpContext.User.Claims.FirstOrDefault(d => string.Equals(d.Type, Claims.SystemId, StringComparison.InvariantCultureIgnoreCase))?.Value,
            RequesterSystemName = HttpContext.User.Claims.FirstOrDefault(d => string.Equals(d.Type, Claims.SystemName, StringComparison.InvariantCultureIgnoreCase))?.Value,
            RequesterUserId = GetUserId(),
            TargetUserId = targetUserId,
            EventType = $"{logEvent}_{suffix}",
            Message = LogEventMessages.GetLogEventMessage(logEvent, suffix),
            EventPayload = payload
        });
    }

    protected IActionResult BadRequestWithAuditLog(IValidatableRequest request, LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, string? targetUserId = null)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (eventPayload is null)
        {
            throw new ArgumentNullException(nameof(eventPayload));
        }

        var validationResult = request.GetValidationResult();

        var msd = new ModelStateDictionary();

        validationResult?.Errors?.ForEach(error =>
        {
            msd.AddModelError(error.PropertyName, error.ErrorMessage);
        });

        eventPayload.Add("Reason", string.Join(",", validationResult.GetValidationErrorList()));
        eventPayload.Add("ResponseStatusCode", HttpStatusCode.BadRequest);
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, targetUserId: targetUserId, payload: eventPayload);

        return ValidationProblem(msd);
    }

    protected IActionResult Result(ServiceResult data, Action<string?, LogEventLifecycle, HttpStatusCode?> auditLog)
    {
        if (data is null)
        {
            auditLog("Argument null exception", LogEventLifecycle.FAIL, null);
            throw new ArgumentNullException(nameof(data));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            auditLog(null, LogEventLifecycle.SUCCESS, null);
            return StatusCode((int)data.StatusCode);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            auditLog(string.Join(",", data?.Errors?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>()), LogEventLifecycle.FAIL, data.StatusCode);
            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        auditLog(data.Error, LogEventLifecycle.FAIL, data.StatusCode);
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected IActionResult Result<T>(ServiceResult<T> data, Action<string?, LogEventLifecycle, HttpStatusCode?> auditLog)
    {
        if (data is null)
        {
            auditLog("Argument null exception", LogEventLifecycle.FAIL, null);
            throw new ArgumentNullException(nameof(data));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            auditLog(null, LogEventLifecycle.SUCCESS, null);
            return StatusCode((int)data.StatusCode, data.Result);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            auditLog(string.Join(",", data?.Errors?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>()), LogEventLifecycle.FAIL, data.StatusCode);
            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        auditLog(data.Error, LogEventLifecycle.FAIL, data.StatusCode);
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected IActionResult ResultWithAuditLog(ServiceResult data, LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, string? targetUserId = null)
    {
        if (data is null)
        {
            eventPayload["Reason"] = "Argument null exception";
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);

            throw new ArgumentNullException(nameof(data));
        }

        if (eventPayload is null)
        {
            eventPayload["Reason"] = "Argument null exception";
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
            throw new ArgumentNullException(nameof(eventPayload));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload, targetUserId: targetUserId);
            return StatusCode((int)data.StatusCode);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            eventPayload["Reason"] = string.Join(",", data?.Errors?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>());
            eventPayload["ResponseStatusCode"] = data.StatusCode;
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        eventPayload["Reason"] = data.Error;
        eventPayload["ResponseStatusCode"] = data.StatusCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected IActionResult ResultWithAuditLog<T>(ServiceResult<T> data, LogEventCode logEventCode, SortedDictionary<string, object> eventPayload, string? targetUserId = null)
    {
        if (data is null)
        {
            eventPayload["Reason"] = "Argument null exception";
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);

            throw new ArgumentNullException(nameof(data));
        }

        if (eventPayload is null)
        {
            eventPayload["Reason"] = "Argument null exception";
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
            throw new ArgumentNullException(nameof(eventPayload));
        }

        // < 400
        if (data.StatusCode < HttpStatusCode.BadRequest)
        {
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.SUCCESS, payload: eventPayload, targetUserId: targetUserId);
            return StatusCode((int)data.StatusCode, data.Result);
        }

        // 400 to < 500
        if (data.StatusCode >= HttpStatusCode.BadRequest && data.StatusCode < HttpStatusCode.InternalServerError)
        {
            var messages = new ModelStateDictionary();

            data.Errors?.ForEach(error =>
            {
                messages.AddModelError(error.Key, error.Value);
            });

            eventPayload["Reason"] = string.Join(",", data?.Errors?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>());
            eventPayload["ResponseStatusCode"] = data.StatusCode;
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
            return ValidationProblem(
                statusCode: (int)data.StatusCode,
                modelStateDictionary: messages);
        }

        // >= 500
        eventPayload["Reason"] = data.Error;
        eventPayload["ResponseStatusCode"] = data.StatusCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload, targetUserId: targetUserId);
        return Problem(
            statusCode: (int)data.StatusCode,
            detail: data.Error
            );
    }

    protected SortedDictionary<string, object> BeginAuditLog(LogEventCode logEventCode, params (string Key, object Value)[] additionalEventPayloadData)
        => BeginAuditLog(logEventCode, null, null, additionalEventPayloadData);

    protected SortedDictionary<string, object> BeginAuditLog(LogEventCode logEventCode, object? request, params (string Key, object Value)[] additionalEventPayloadData)
        => BeginAuditLog(logEventCode, request, null, additionalEventPayloadData);

    protected SortedDictionary<string, object> BeginAuditLog(LogEventCode logEventCode, object? request, string? targetUserId, params (string Key, object Value)[] additionalEventPayloadData)
    {
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() },
            { AuditLoggingKeys.RequesterName, GetUserFullName() }
        };

        if (request != null)
        {
            eventPayload[AuditLoggingKeys.Request] = request;
        }

        if (additionalEventPayloadData != null)
        {
            foreach (var (Key, Value) in additionalEventPayloadData)
            {
                eventPayload[Key] = Value;
            }
        }

        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, LogEventLifecycle.REQUEST, targetUserId, payload: eventPayload);

        return eventPayload;
    }
}

public class SetRequestIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Set Http request header Id
        if (!Guid.TryParse(context.HttpContext.Request.Headers.RequestId, out var requestId))
        {
            requestId = InVar.CorrelationId;
            context.HttpContext.Request.Headers.RequestId = requestId.ToString();
        }

        // Set requestId as a local value
        var controller = (BaseV1Controller)context.Controller;
        controller.RequestId = requestId;
        controller.Logger.BeginScope("{RequestId}", requestId);

        base.OnActionExecuting(context);
    }
}

