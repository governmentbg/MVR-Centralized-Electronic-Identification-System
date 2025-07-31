namespace eID.RO.API.Public.Options;

public class ApplicationUrls
{
    public string PdeauHostUrl { get; set; } = string.Empty;
    public string KeycloakHostUrl { get; set; } = string.Empty;
    public string SigningHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PdeauHostUrl))
        {
            throw new ArgumentNullException(nameof(PdeauHostUrl));
        }

        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }

        if (string.IsNullOrWhiteSpace(SigningHostUrl))
        {
            throw new ArgumentNullException(nameof(SigningHostUrl));
        }
    }
}
