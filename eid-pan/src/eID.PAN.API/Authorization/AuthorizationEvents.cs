using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace eID.PAN.API.Authorization;

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
        if (principal == null)
        {
            return Task.CompletedTask;
        }

        var realmAccess = principal.FindFirst("realm_access")?.Value;
        if (realmAccess == null || principal.Identity is not ClaimsIdentity claimsIdentity)
        {
            return Task.CompletedTask;
        }

        try
        {
            var parsed = JsonConvert.DeserializeObject<RealmAccess>(realmAccess);
            if (parsed?.Roles == null)
            {
                return Task.CompletedTask;
            }
            
            foreach (var role in parsed.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        catch (JsonException ex)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtBearerEvents_OnTokenValidated");
            logger.LogError(ex, "Failed to parse realm_access from token.");
        }

        return Task.CompletedTask;
    }

    private class RealmAccess
    {
        [JsonProperty("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
