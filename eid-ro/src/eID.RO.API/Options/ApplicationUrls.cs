namespace eID.RO.API.Options;

public class ApplicationUrls
{
    public string KeycloakHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
    }
}
