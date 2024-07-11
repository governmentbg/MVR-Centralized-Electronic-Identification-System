namespace eID.RO.Service.Options;

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
            throw new ArgumentNullException(nameof(BaseUrl));
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
