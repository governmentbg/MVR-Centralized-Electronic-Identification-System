using System.Net.Mail;

namespace eID.PDEAU.Service.Options;

public class NotificationEmailsOptions
{
    public string DEAUActions { get; set; } = string.Empty;
    
    public void IsValid()
    {
        if (!IsValidEmail(DEAUActions))
        {
            throw new InvalidOperationException($"Invalid {nameof(NotificationEmailsOptions)}.{nameof(DEAUActions)} email: '{DEAUActions}'");
        }
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Use MailAddress class to validate email format
            var addr = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
