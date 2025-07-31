namespace eID.MIS.API.Options;

public class ApplicationUrls
{
    public string IntegrationsHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(IntegrationsHostUrl))
        {
            throw new ArgumentNullException(nameof(IntegrationsHostUrl));
        }
        var validUri = Uri.TryCreate(IntegrationsHostUrl, UriKind.Absolute, out Uri? validatedUri);
        var validScheme = (validatedUri?.Scheme == Uri.UriSchemeHttp || validatedUri?.Scheme == Uri.UriSchemeHttps);
        if (!validUri)
        {
            throw new ArgumentException("Invalid Uri", nameof(IntegrationsHostUrl));
        }
        if (!validScheme)
        {
            throw new ArgumentException("Invalid Uri scheme", nameof(IntegrationsHostUrl));
        }
    }
}
