using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace eID.PIVR.API.Authorization;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RoleAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _requiredRoles;

    public RoleAuthorizationAttribute(params string[] roles)
    {
        _requiredRoles = roles;
    }

    public string[] Roles => _requiredRoles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var skipCheck = context.Filters.OfType<SkipRoleCheckAttribute>().Any() ||
                        context.ActionDescriptor.EndpointMetadata.OfType<SkipRoleCheckAttribute>().Any() ||
                        context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (skipCheck)
        {
            return;
        }

        var logger = context.HttpContext.RequestServices.GetService<ILogger<RoleAuthorizationAttribute>>();
        var user = context.HttpContext.User;
        var rolesClaim = user.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
        var roles = new HashSet<string>();

        if (!string.IsNullOrEmpty(rolesClaim))
        {
            try
            {
                var parsedRoles = JsonConvert.DeserializeObject<RealmAccess>(rolesClaim);
                if (parsedRoles?.Roles != null)
                {
                    roles = new HashSet<string>(parsedRoles.Roles);
                }
            }
            catch
            {
                logger?.LogWarning("Invalid realm_access format in JWT.");
            }
        }

        // Forbid request if roles in token are empty
        if (!roles.Any())
        {
            logger?.LogWarning("Access denied: No roles found in token.");
            context.Result = new ForbidResult();
            return;
        }

        var clientIdClaim = user.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
        bool isServerRequest = !string.IsNullOrEmpty(clientIdClaim) && clientIdClaim.EndsWith("_m2m");
        if (isServerRequest)
        {
            // If it's a server request, we check if it is one of the AllowedServers.
            var settings = context.HttpContext.RequestServices.GetService<AuthorizationSettings>();
            if (settings.AllowedServers.Any() && !roles.Any(r => settings.AllowedServers.Contains(r)))
            {
                logger?.LogWarning($"Server access denied: {clientIdClaim} does not have an allowed role.");
                context.Result = new ForbidResult();
            }
            return;
        }

        // Collecting method attributes
        var methodAttributes = context.ActionDescriptor.EndpointMetadata
            .OfType<RoleAuthorizationAttribute>();

        var allRequiredRoles = methodAttributes
            .SelectMany(q => q.Roles.Select(r => r))
            .Distinct()
            .ToList();

        if (!allRequiredRoles.Any())
        {
            // there's nothing to check
            return;
        }

        if (allRequiredRoles.Any() && !roles.Any(r => allRequiredRoles.Contains(r)))
        {
            logger?.LogWarning("Access denied: Missing required role.");
            context.Result = new ForbidResult();
        }
    }

    private class RealmAccess
    {
        public string[] Roles { get; set; }
    }
}
