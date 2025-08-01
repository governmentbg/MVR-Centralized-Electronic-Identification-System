using System.Security.Policy;

namespace eID.PDEAU.Service.Options;

public class TimestampServerOptions
{
    public static readonly string HTTP_CLIENT_NAME = "TimestampServer";
    public string BaseUrl { get; set; } = string.Empty;
    public string RequestTokenUrl { get; set; } = string.Empty;
    public string CertificatePath { get; set; } = string.Empty;
    public string? CertificatePass { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("Value must not be empty", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var _))
        {
            throw new ArgumentException($"Value: '{BaseUrl}' must be valid Uri", nameof(BaseUrl));
        }

        if (string.IsNullOrWhiteSpace(RequestTokenUrl))
        {
            throw new ArgumentNullException(nameof(RequestTokenUrl));
        }
        
        if (string.IsNullOrWhiteSpace(CertificatePath))
        {
            throw new ArgumentNullException(nameof(CertificatePath));
        }
    }
}
