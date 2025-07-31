namespace eID.PDEAU.Service.Options;

public class ApplicationUrls
{
    public string PivrHostUrl { get; set; } = string.Empty;
    public string PanHostUrl { get; set; } = string.Empty;
    public string KeycloakHostUrl { get; set; } = string.Empty;
    public string PdeauHostUrl { get; set; } = string.Empty;
    public string MpozeiHostUrl { get; set; } = string.Empty;
    public string PdeauUiUrl { get; set; } = string.Empty;

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

        if (string.IsNullOrWhiteSpace(PanHostUrl))
        {
            throw new ArgumentNullException(nameof(PanHostUrl));
        }

        if (string.IsNullOrWhiteSpace(PdeauHostUrl))
        {
            throw new ArgumentNullException(nameof(PdeauHostUrl));
        }

        if (string.IsNullOrWhiteSpace(MpozeiHostUrl))
        {
            throw new ArgumentNullException(nameof(MpozeiHostUrl));
        }

        if (string.IsNullOrWhiteSpace(PdeauUiUrl))
        {
            throw new ArgumentNullException(nameof(PdeauUiUrl));
        }
    }
}
