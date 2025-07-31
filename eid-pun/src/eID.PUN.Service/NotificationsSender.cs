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
        var allResults = new List<bool>();
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
            allResults.Add(sendResult);
        }
        if (allResults.Any(success => !success))
        {
            _logger.LogInformation(
                "Out of {TotalCount} notifications {SuccessfulCount} were sent successfully and {FailedCount} failed.",
                    allResults.Count,
                    allResults.Count(success => success),
                    allResults.Count(success => !success)
                );
        }
        else
        {
            _logger.LogInformation("All notifications has been send successfully.");
        }
        return allNotifactionsSendSuccessfully;
    }

    private async Task<bool> SendOneNotificationAsync(SendNotificationRequest notification)
    {
        var sendNotificationUrl = "/api/v1/notifications/send";
        var httpSendNotificationBody = BuildSendNotificationHttpBody(notification);

        _logger.LogInformation("Attempting to send notification to {EId}", notification.EId);

        try
        {
            var statusesWeWontRetry = new System.Net.HttpStatusCode[] {
                System.Net.HttpStatusCode.BadRequest,
                System.Net.HttpStatusCode.Conflict,
                System.Net.HttpStatusCode.InternalServerError,
                System.Net.HttpStatusCode.NotFound,
                System.Net.HttpStatusCode.Unauthorized,
                System.Net.HttpStatusCode.Forbidden
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
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed event registration. StatusCode: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode, exception.Result?.ToString(), DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.PostAsync(
                $"{sendNotificationUrl}",
                new StringContent(JsonConvert.SerializeObject(httpSendNotificationBody), Encoding.UTF8, "application/json")
            ));

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Send notification to {EId} failed response: {Response}", notification.EId, responseBody);
                return false;
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to send notification to {EId}. Request: POST {Url}, Body {Body}", notification.EId, sendNotificationUrl, httpSendNotificationBody);
            return false;
        }

        _logger.LogInformation("Sent notification to {EId}", notification.EId);
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
