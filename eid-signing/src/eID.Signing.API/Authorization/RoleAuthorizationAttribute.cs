using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eID.Signing.API.Authorization;

/// <summary>
/// Custom <see cref="AuthorizeAttribute"/> which allow and forbid described in the constructor roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RoleAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;
    private readonly string[] _forbiddenRoles;
    private readonly bool _allowM2M;

    /// <summary>
    /// Crate an instance of <see cref="RoleAuthorizationAttribute"/> class
    /// </summary>
    /// <param name="allowedRoles">Allowed roles, separated by comma. OR rule is applied.</param>
    /// <param name="forbiddenRoles">Forbidden roles, separated by comma. OR rule is applied.</param>
    /// <param name="allowM2M">True if machine to machine call is allowed</param>
    public RoleAuthorizationAttribute(string allowedRoles, string forbiddenRoles = "", bool allowM2M = false)
    {
        _allowedRoles = allowedRoles.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _forbiddenRoles = forbiddenRoles.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _allowM2M = allowM2M;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        // Forbidden roles are with high priority. They are checked first.
        var isInForbiddenRole = user.Claims.Where(c => c.Type == ClaimTypes.Role).Any(c => _forbiddenRoles.Contains(c.Value));
        if (isInForbiddenRole)
        {
            // Check if forbidden role is allowed with AllowForbiddenRolesAttribute.
            var allowedForbiddenRoles = context.ActionDescriptor.EndpointMetadata.OfType<AllowForbiddenRolesAttribute>().FirstOrDefault()?.AllowedRoles;
            if (allowedForbiddenRoles != null && allowedForbiddenRoles.Any(fr => _forbiddenRoles.Contains(fr)))
            {
                return;
            }

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RoleAuthorizationAttribute>>();
            logger.LogWarning("Access denied: The user's role is not permitted.");
            
            context.Result = new ForbidResult();
            return;
        }

        // Is a part of allowed roles or AllowAnonymous?
        var isInAllowedRoles = 
                context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() ||
                user.Claims.Where(c => c.Type == ClaimTypes.Role).Any(c => _allowedRoles.Contains(c.Value));
        if (isInAllowedRoles)
        {
            return;
        }

        // Check for allowing m2m call
        if (_allowM2M)
        {
            var autorizedPartyValue = user.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
            var isM2MRequest = !string.IsNullOrEmpty(autorizedPartyValue) && autorizedPartyValue.EndsWith("_m2m");
            if (isM2MRequest)
            {
                return;
            }
        }

        // All other roles are forbidden.
        context.Result = new ForbidResult();
    }
}
