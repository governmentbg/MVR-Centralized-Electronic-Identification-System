using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eID.PAN.API.Authorization;

/// <summary>
/// Custom <see cref="AuthorizeAttribute"/> which allow and forbid described in the constructor roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RoleAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;
    private readonly string[] _forbiddenRoles;
    private readonly bool _allowM2M;

    /// <summary>
    /// Allowed roles
    /// </summary>
    public string[] AllowedRoles => _allowedRoles;

    /// <summary>
    /// Forbidden roles
    /// </summary>
    public string[] ForbiddenRoles => _forbiddenRoles;
    
    /// <summary>
    /// Allow machine to machine communication (m2m)
    /// </summary>
    public bool AllowM2M => _allowM2M;

    /// <summary>
    /// Crate an instance of <see cref="RoleAuthorizationAttribute"/> class
    /// </summary>
    /// <param name="allowedRoles">Allowed roles, separated by comma. OR rule is applied.</param>
    /// <param name="forbiddenRoles">Forbidden roles, separated by comma. OR rule is applied.</param>
    /// <param name="allowM2M">True if machine to machine call is allowed</param>
    public RoleAuthorizationAttribute(string allowedRoles = "", string forbiddenRoles = "", bool allowM2M = false)
    {
        _allowedRoles = allowedRoles.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _forbiddenRoles = forbiddenRoles.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _allowM2M = allowM2M;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var userRoles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        var methodAttributes = context.ActionDescriptor.EndpointMetadata
            .OfType<RoleAuthorizationAttribute>();
        
        // Collect all forbidden roles
        var allForbiddenRoles = methodAttributes
            .SelectMany(q => q.ForbiddenRoles)
            .ToHashSet();

        // Forbidden roles are with high priority. They are checked first.
        var isInForbiddenRole = userRoles.Any(r => allForbiddenRoles.Contains(r));
        if (isInForbiddenRole)
        {
            // Check if forbidden roles are allowed with AllowForbiddenRolesAttribute.
            var allowedForbiddenRoles = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowForbiddenRolesAttribute>()?
                .SelectMany(ar => ar.AllowedRoles)
                .ToArray();
            
            if (allowedForbiddenRoles != null && allowedForbiddenRoles.Any(fr => allForbiddenRoles.Contains(fr)))
            {
                return;
            }

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RoleAuthorizationAttribute>>();
            logger.LogWarning("Access denied: The user's role is not permitted.");

            context.Result = new ForbidResult();
            return;
        }

        // Collect all allowed roles
        var allAllowedRoles = methodAttributes
            .SelectMany(q => q.AllowedRoles)
            .ToHashSet();

        // Is a part of allowed roles or AllowAnonymous?
        var isInAllowedRoles =
                context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() ||
                userRoles.Any(r => allAllowedRoles.Contains(r));
        if (isInAllowedRoles)
        {
            return;
        }

        // Collect all allowed m2m call
        var allowedM2MCall = methodAttributes.Any(q => q.AllowM2M);
        if (allowedM2MCall)
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
