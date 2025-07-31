using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace eID.POD.API.Authorization;

internal static class AuthorizationEvents
{
    /// <summary>
    /// Convert Keycloak roles in recognizable for <seealso cref="AuthorizeAttribute"/>
    /// </summary>
    /// <param name="context">Token context</param>
    /// <returns><seealso cref="Task"/></returns>
    public static Task OnTokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal;
        if (principal != null)
        {
            var realmAccess = principal.FindFirst("realm_access")?.Value;
            if (realmAccess != null && principal.Identity is ClaimsIdentity claimsIdentity)
            {
                try
                {
                    var parsed = System.Text.Json.JsonDocument.Parse(realmAccess);
                    if (parsed.RootElement.TryGetProperty("roles", out var rolesElement))
                    {
                        var roles = rolesElement.EnumerateArray().Select(r => r.GetString());
                        foreach (var role in roles)
                        {
                            if (!string.IsNullOrWhiteSpace(role))
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogError("Invalid realm_access format in JWT. Error: {error}", ex.Message);
                }
            }
        }

        return Task.CompletedTask;
    }
}
