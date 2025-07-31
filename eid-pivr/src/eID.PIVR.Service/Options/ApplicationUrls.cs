namespace eID.PIVR.Service.Options;

public class ApplicationUrls
{
    public string IntegrationsHostUrl { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(IntegrationsHostUrl))
        {
            throw new InvalidOperationException($"{nameof(IntegrationsHostUrl)} is null or empty");
        }
    }
}
