namespace eID.Signing.Application.Options;

public class ApplicationUrls
{
    public string EvrotrustHostUrl { get; set; } = string.Empty;
    public string BoricaHostUrl { get; set; } = string.Empty;
    public string KEPUrl { get; set; } = string.Empty;
    public string PanHostUrl { get; set; } = string.Empty;
    public string KeycloakHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(EvrotrustHostUrl))
        {
            throw new ArgumentNullException(nameof(EvrotrustHostUrl));
        }

        if (string.IsNullOrWhiteSpace(BoricaHostUrl))
        {
            throw new ArgumentNullException(nameof(BoricaHostUrl));
        }

        if (string.IsNullOrWhiteSpace(KEPUrl))
        {
            throw new ArgumentNullException(nameof(KEPUrl));
        }

        if (string.IsNullOrWhiteSpace(PanHostUrl))
        {
            throw new ArgumentNullException(nameof(PanHostUrl));
        }

        if (string.IsNullOrWhiteSpace(KeycloakHostUrl))
        {
            throw new ArgumentNullException(nameof(KeycloakHostUrl));
        }
    }
}
