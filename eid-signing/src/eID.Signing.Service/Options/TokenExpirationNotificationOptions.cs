using System.Net.Mail;

namespace eID.Signing.Service.Options;

public class TokenExpirationNotificationOptions
{
    public string Email { get; set; } = string.Empty;
    
    public string Subject { get; set; } = string.Empty;

    public void Validate()
    {
        if (!IsValidEmail(Email))
        {
            throw new InvalidOperationException($"Invalid {nameof(TokenExpirationNotificationOptions)}.{nameof(Email)} email: '{Email}'");
        }

        if (string.IsNullOrWhiteSpace(Subject))
        {
            throw new ArgumentNullException(nameof(Subject));
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
