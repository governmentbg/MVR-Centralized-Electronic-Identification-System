namespace eID.PDEAU.API.Options;

public class ApplicationUrls
{
    public string PivrHostUrl { get; set; } = string.Empty;
    public string KeycloakHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PivrHostUrl))
        {
            throw new ArgumentNullException(nameof(PivrHostUrl));
        }

        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
    }
}
