namespace eID.PAN.Service.Options;

public class SmtpOptions
{
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SenderName))
        {
            throw new ArgumentNullException(nameof(SenderName));
        }

        if (string.IsNullOrWhiteSpace(SenderEmail))
        {
            throw new ArgumentNullException(nameof(SenderEmail));
        }
    }
}
