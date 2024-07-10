namespace eID.POD.Service.Options;

public class OpenDataSettings
{
    public bool AutomaticStart { get; set; }
    public string OpenDataApiKey { get; set; } = string.Empty;
    public string OpenDataUrl { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public int CategoryId { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(OpenDataApiKey))
        {
            throw new ArgumentNullException(nameof(OpenDataApiKey));
        }

        if (string.IsNullOrWhiteSpace(OpenDataUrl))
        {
            throw new ArgumentNullException(nameof(OpenDataUrl));
        }

        if (!Uri.TryCreate(OpenDataUrl, UriKind.Absolute, out var _))
        {
            throw new ArgumentException(nameof(OpenDataUrl), $"{nameof(OpenDataUrl)} is not absolute url.");
        }

        if (OrganizationId <= 0)
        {
            throw new ArgumentNullException(nameof(OrganizationId));
        }

        if (CategoryId <= 0)
        {
            throw new ArgumentNullException(nameof(CategoryId));
        }
    }
}
