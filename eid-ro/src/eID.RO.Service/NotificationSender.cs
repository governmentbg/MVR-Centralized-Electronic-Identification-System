using System.Text;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Service.Requests;
using eID.RO.Service.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace eID.RO.Service;

public class NotificationSender : INotificationSender
{
    private readonly ILogger<NotificationSender> _logger;
    private readonly HttpClient _httpClient;

    public NotificationSender(
        ILogger<NotificationSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
    }

    public async Task<bool> SendAsync(NotifyUids notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        // Validation
        var validator = new NotifyUidsValidator();
        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Notification Sender validation failed. {Errors}", validationResult.Errors);
            return false;
        }

        var tasks = notification.Uids
            .Select(uid => SendSingleNotificationAsync(new SendNotificationRequest
            {
                RequestId = notification.CorrelationId.ToString(),
                EventCode = notification.EventCode,
                Uid = uid.Uid,
                UidType = uid.UidType,
                Translations = notification.Translations
            }))
            .ToList();
        await Task.WhenAll(tasks);

        return tasks
                .Select(t => t.Result)
                .All(result => result == true);
    }

    public async Task<bool> SendAsync(NotifyUid notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        // Validation
        var validator = new NotifyUidValidator();
        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Notification Sender validation failed. {Errors}", validationResult.Errors);
            return false;
        }

        var result = await SendSingleNotificationAsync(new SendNotificationRequest
        {
            RequestId = notification.CorrelationId.ToString(),
            EventCode = notification.EventCode,
            Uid = notification.Uid,
            UidType = notification.UidType
        });

        return result;
    }

    private async Task<bool> SendSingleNotificationAsync(SendNotificationRequest notification)
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
                                    "Failed sending notification. StatusCode: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode, exception.Result?.ToString(), DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(async () =>
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, sendNotificationUrl);
                requestMessage.Headers.TryAddWithoutValidation("Request-Id", notification.RequestId);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(httpSendNotificationBody), 
                    Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
                
                return await _httpClient.SendAsync(requestMessage);
            });

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Send notification failed response: ({StatusCode}) {Response}", response.StatusCode, responseBody);
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to send notification. Request: POST {Url}, Body {Body}", sendNotificationUrl, httpSendNotificationBody);
            return false;
        }
        _logger.LogInformation("Send notification request completed successfully.");
        return true;
    }

    private static object BuildSendNotificationHttpBody(SendNotificationRequest notification)
    {
        var body = new
        {
            notification.EventCode,
            notification.Uid,
            notification.UidType,
            // Mapping the translations to the expected format
            Translations = notification.Translations.Select(t => new { t.Language, Message = t.Description })
        };

        return body;
    }
}

internal class SendNotificationRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public IEnumerable<Translation> Translations { get; set; } = Enumerable.Empty<Translation>();
}

public interface INotificationSender
{
    Task<bool> SendAsync(NotifyUids notification);
    Task<bool> SendAsync(NotifyUid notification);
}
