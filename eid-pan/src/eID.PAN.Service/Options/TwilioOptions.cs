namespace eID.PAN.Service.Options;

public class TwilioOptions
{
    public string? AccountSid { get; set; }
    public string? AuthToken { get; set; }
    public string? FromPhoneNumber { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountSid))
        {
            throw new ArgumentNullException(nameof(AccountSid));
        }

        if (string.IsNullOrWhiteSpace(AuthToken))
        {
            throw new ArgumentNullException(nameof(AuthToken));
        }

        if (string.IsNullOrWhiteSpace(FromPhoneNumber))
        {
            throw new ArgumentNullException(nameof(FromPhoneNumber));
        }
    }
}
