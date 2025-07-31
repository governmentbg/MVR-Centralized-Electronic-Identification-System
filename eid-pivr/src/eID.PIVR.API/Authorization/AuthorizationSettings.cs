namespace eID.PIVR.API.Authorization;

public class AuthorizationSettings
{
    public HashSet<string> AllowedServers { get; }

    public AuthorizationSettings(IConfiguration configuration)
    {
        var allowedServers = configuration.GetSection("AllowedServers").Get<string[]>();
        AllowedServers = allowedServers != null ? new HashSet<string>(allowedServers) : new HashSet<string>();
    }
}
