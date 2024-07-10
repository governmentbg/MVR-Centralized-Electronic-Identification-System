using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Polly;

namespace eID.PAN.Service;

public class PushNotificationSender : IPushNotificationSender
{
    private readonly ILogger<PushNotificationSender> _logger;
    public PushNotificationSender(ILogger<PushNotificationSender> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public virtual async Task<bool> SendPushNotificationAsync(Guid userId, IReadOnlyList<string> tokens, string localizedMessageBody)
    {
        if (tokens is null)
        {
            throw new ArgumentNullException(nameof(tokens));
        }

        if (!tokens.Any())
        {
            throw new ArgumentException($"'{nameof(tokens)}' cannot be empty.", nameof(tokens));
        }

        if (string.IsNullOrEmpty(localizedMessageBody))
        {
            throw new ArgumentException($"'{nameof(localizedMessageBody)}' cannot be null or empty.", nameof(localizedMessageBody));
        }

        var notification = new MulticastMessage()
        {
            Tokens = tokens,
            Notification = new Notification
            {
                //Title = "TBD",
                Body = localizedMessageBody
            },
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    DefaultSound = true,
                    DefaultVibrateTimings = true
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    ContentAvailable = true,
                    Sound = "default"
                }
            },
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Silent = false
                }
            }
        };

        _logger.LogInformation("Sending push notifications to {NumberOfTokens} devices for user {UserId}", tokens.Count, userId);
        var retryPolicy = Policy
            .Handle<FirebaseMessagingException>(ex =>
                MessagingErrorCode.Unavailable == ex.MessagingErrorCode
            )
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (ex, ts) =>
                {
                    _logger.LogWarning("Retry sending push notification. Reason: {ExceptionMessage}", ex.Message);
                });
        try
        {
            var response = await retryPolicy.ExecuteAsync(() => FirebaseMessaging.DefaultInstance.SendMulticastAsync(notification));
            if (response.FailureCount > 0)
            {
                _logger.LogInformation("Failed sending notifications to {FailedCount} devices.", response.FailureCount);
                foreach (var failedNotification in response.Responses.Where(x => !x.IsSuccess))
                {
                    _logger.LogDebug("{MessagingErrorCode} {ExceptionMessage}", failedNotification.Exception.MessagingErrorCode, failedNotification.Exception.Message);
                }
            }

            if (response.SuccessCount > 0)
            {
                _logger.LogInformation("Successfully sent push notification to {DeviceCount}/{TotalDevices} devices of {UserId}", response.SuccessCount, response.Responses.Count, userId);
            }
            else
            {
                _logger.LogInformation("No notifications were sent to {UserId}'s devices.", userId);
            }

            return response.SuccessCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed sending notifications to {UserId}'s devices.", userId);
            return false;
        }
    }
}

public interface IPushNotificationSender
{
    Task<bool> SendPushNotificationAsync(Guid userId, IReadOnlyList<string> tokens, string localizedMessageBody);
}
