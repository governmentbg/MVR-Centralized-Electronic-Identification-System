using Asp.Versioning;
using eID.PJS.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eID.PJS.Services.Controllers;


/// <summary>
/// Base controller
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[ApiController]
[Authorize]
[RoleAuthorization(UserRoles.ApplicationAdministrator, UserRoles.DCISAdministrator)]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
[ProducesResponseType(StatusCodes.Status504GatewayTimeout, Type = typeof(ProblemDetails))]
public class BaseV1Controller : ControllerBase
{

    internal ILogger Logger { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseV1Controller"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <exception cref="System.ArgumentNullException">logger</exception>
    public BaseV1Controller(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the bad requests and returns the errors.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">request</exception>
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

    protected string GetUserId() =>
       HttpContext.User.Claims.FirstOrDefault(d => d.Type == "USERID")?.Value ?? "Unknown user";
}


