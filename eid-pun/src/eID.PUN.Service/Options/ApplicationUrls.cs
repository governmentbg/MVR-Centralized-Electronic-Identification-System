namespace eID.PUN.Service.Options;

public class ApplicationUrls
{
    public string PanHostUrl { get; set; } = string.Empty;
    public string PanPublicHostUrl { get; set; } = string.Empty;
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
        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
    }
}
