using System.Text;
using eID.PUN.Contracts.Commands;
using eID.PUN.Service.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace eID.PUN.Service;

public class NotificationsSender : INotificationsSender
{
    private readonly ILogger<NotificationsSender> _logger;
    private readonly HttpClient _httpClient;

    public NotificationsSender(
        ILogger<NotificationsSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
    }

    public async Task<bool> SendAsync(NotifyEIds message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new NotifyEIdsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Notification Sender validation failed. {Errors}", validationResult.Errors);
            return false;
        }

        var allNotifactionsSendSuccessfully = true;
        foreach (var eId in message.EIds)
        {
            var notification = new SendNotificationRequest
            {
                EventCode = message.EventCode,
                EId = eId,
                Translations = message.Translations
            };

            var sendResult = await SendOneNotificationAsync(notification);

            if (!sendResult)
            {
                allNotifactionsSendSuccessfully = false;
            }
        }

        _logger.LogInformation("All notifications has been send successfully.");
        return allNotifactionsSendSuccessfully;
    }

    private async Task<bool> SendOneNotificationAsync(SendNotificationRequest notification)
    {
        var sendNotificationUrl = "/api/v1/notifications/send";
        var httpSendNotificationBody = BuildSendNotificationHttpBody(notification);

        _logger.LogInformation("Attempting to send notification...");

        try
        {
            var statusesWeWontRetry = new System.Net.HttpStatusCode[] {
                System.Net.HttpStatusCode.BadRequest,
                System.Net.HttpStatusCode.Conflict,
                System.Net.HttpStatusCode.InternalServerError,
                System.Net.HttpStatusCode.NotFound
            };
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse =>
                                        !httpResponse.IsSuccessStatusCode
                                            && !statusesWeWontRetry.Contains(httpResponse.StatusCode))
                            .WaitAndRetryForeverAsync(
                            _ => TimeSpan.FromSeconds(60),
                            (exception, timespan) =>
                            {
                                _logger.LogInformation("Failed sending notification. Next attempt will be at {NextAttemptTime}", DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.PostAsync(
                $"{sendNotificationUrl}?testSystemName=PUN", // TODO: Remove query parameter once we have S2S auth
                new StringContent(JsonConvert.SerializeObject(httpSendNotificationBody), Encoding.UTF8, "application/json")
            ));

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Send notification failed response: {Response}", responseBody);
                return false;
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to send notification. Request: POST {Url}, Body {Body}", sendNotificationUrl, httpSendNotificationBody);
            return false;
        }

        return true;
    }

    private static object BuildSendNotificationHttpBody(SendNotificationRequest notification)
    {
        var body = new
        {
            notification.EventCode,
            notification.EId,
            // Mapping the translations to the expected format
            Translations = notification.Translations.Select(t => new { t.Language, Message = t.Description })
        };

        return body;
    }
}

internal class SendNotificationRequest
{
    public string EventCode { get; set; }
    public Guid EId { get; set; }
    public IEnumerable<Translation> Translations { get; set; }
}

public interface INotificationsSender
{
    Task<bool> SendAsync(NotifyEIds notification);
}
