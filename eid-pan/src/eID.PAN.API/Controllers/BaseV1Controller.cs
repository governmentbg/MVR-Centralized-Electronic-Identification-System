using System.Net;
using Asp.Versioning;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Results;
using eID.PJS.AuditLogging;

using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eID.PAN.API.Controllers;

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
    private readonly IConfiguration _configuration;
    private readonly AuditLogger _auditLogger;

    internal ILogger Logger { get; private set; }

    public BaseV1Controller(IConfiguration configuration, ILogger logger, AuditLogger auditLogger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    }

    public Guid RequestId { get; set; }

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

    protected string GetUserId() =>
        HttpContext.User.Claims.FirstOrDefault(d => d.Type == "USERID")?.Value ?? "Unknown user";

    protected string GetSystemName() =>
        HttpContext.User.Claims.FirstOrDefault(d => d.Type == "systemName")?.Value ?? "Unknown system";    

    protected void AddAuditLog(LogEventCode logEvent, string? targetUserId = default, string? message = default, SortedDictionary<string, object>? payload = default)
    {
        if (_configuration.GetValue<bool>("SkipAuditLogging"))
        {
            return;
        }

        //TODO: Define propper values when the new logger version is available
        _auditLogger.LogEvent(new AuditLogEvent
        {
            CorrelationId = Guid.NewGuid().ToString(),
            RequesterSystemId = GetUserId(),
            RequesterUserId = GetUserId(),
            TargetUserId = targetUserId,
            EventType = logEvent.ToString(),
            Message = message,
            EventPayload = payload
        });
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
