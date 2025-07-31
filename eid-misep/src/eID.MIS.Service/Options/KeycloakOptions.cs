namespace eID.MIS.Service.Options;

public class KeycloakOptions
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? GrantType { get; set; }
    public string? Realm { get; set; }

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
