using eID.PAN.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace eID.PAN.Service;

public class TwilioSmsSender : ISmsSender
{
    private readonly ILogger<TwilioSmsSender> _logger;
    private readonly TwilioOptions _twilioOptions;

    public TwilioSmsSender(ILogger<TwilioSmsSender> logger, IOptions<TwilioOptions> twilioOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _twilioOptions = (twilioOptions ?? throw new ArgumentNullException(nameof(twilioOptions))).Value;
        _twilioOptions.Validate();
    }

    public async Task<bool> SendSmsAsync(Guid userId, string userPhoneNumber, string messageBodyInSelectedLanguage)
    {
        if (string.IsNullOrWhiteSpace(userPhoneNumber))
        {
            throw new ArgumentException($"'{nameof(userPhoneNumber)}' cannot be null or whitespace.", nameof(userPhoneNumber));
        }

        if (string.IsNullOrWhiteSpace(messageBodyInSelectedLanguage))
        {
            throw new ArgumentException($"'{nameof(messageBodyInSelectedLanguage)}' cannot be null or whitespace.", nameof(messageBodyInSelectedLanguage));
        }

        var smsOptions = new CreateMessageOptions(new PhoneNumber(userPhoneNumber))
        {
            From = new PhoneNumber(_twilioOptions.FromPhoneNumber),
            Body = messageBodyInSelectedLanguage
        };

        //SMS is sent here
        try
        {
            var sms = await MessageResource.CreateAsync(smsOptions);
            if (!string.IsNullOrWhiteSpace(sms.ErrorMessage))
            {
                _logger.LogInformation("Failed to send SMS to {UserId}. Reason: {Reason}", userId, sms.ErrorMessage);
                return false;
            }

            _logger.LogInformation("SMS sent successfully to user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to send SMS to {UserId}.", userId);
            return false;
        }
    }
}

public interface ISmsSender
{
    Task<bool> SendSmsAsync(Guid userId, string userPhoneNumber, string messageBodyInSelectedLanguage);
}
