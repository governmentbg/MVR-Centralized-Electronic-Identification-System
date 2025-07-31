namespace eID.Authorization.Keycloak;

public class KeycloakOptions
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string GrantType { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Realm))
        {
            throw new ArgumentNullException(nameof(Realm));
        }

        if (string.IsNullOrWhiteSpace(GrantType))
        {
            throw new ArgumentNullException(nameof(GrantType));
        }

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new ArgumentNullException(nameof(ClientId));
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new ArgumentNullException(nameof(ClientSecret));
        }
    }
}
