namespace eID.PAN.Service.Options;

public class ApplicationUrls
{
    public string KeycloakHostUrl { get; set; } = string.Empty;
    public string MpozeiHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
        if (string.IsNullOrWhiteSpace(MpozeiHostUrl))
        {
            throw new ArgumentNullException(nameof(MpozeiHostUrl));
        }
    }
}
