namespace eID.RO.Application.Options;

public class ApplicationUrls
{
    public string PanHostUrl { get; set; } = string.Empty;
    public string PanPublicHostUrl { get; set; } = string.Empty;
    public string PivrHostUrl { get; set; } = string.Empty;
    public string IntegrationsHostUrl { get; set; } = string.Empty;
    public string KeycloakHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PanHostUrl))
        {
            throw new ArgumentNullException(nameof(PanHostUrl));
        }

        if (string.IsNullOrWhiteSpace(PanPublicHostUrl))
        {
            throw new ArgumentNullException(nameof(PanPublicHostUrl));
        }

        if (string.IsNullOrWhiteSpace(PivrHostUrl))
        {
            throw new ArgumentNullException(nameof(PivrHostUrl));
        }

        if (string.IsNullOrWhiteSpace(IntegrationsHostUrl))
        {
            throw new ArgumentNullException(nameof(IntegrationsHostUrl));
        }

        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
    }
}
