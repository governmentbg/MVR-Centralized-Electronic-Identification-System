using System.Net.Http.Headers;
using System.Text;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Service.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace eID.RO.Service;

public class NotificationSender : INotificationSender
{
    private readonly ILogger<NotificationSender> _logger;
    private readonly HttpClient _httpClient;
    private readonly IKeycloakCaller _keycloakCaller;

    public NotificationSender(
        ILogger<NotificationSender> logger,
        IHttpClientFactory httpClientFactory,
        IKeycloakCaller keycloakCaller)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
        _keycloakCaller = keycloakCaller ?? throw new ArgumentNullException(nameof(keycloakCaller));
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

    private async Task<bool> SendSingleNotificationAsync(SendNotificationRequest notification)
    {
        var sendNotificationUrl = "/api/v1/notifications/send";
        var httpSendNotificationBody = BuildSendNotificationHttpBody(notification);

        _logger.LogInformation("Attempting to send notification...");

        try
        {
            var keycloakToken = await _keycloakCaller.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(keycloakToken))
            {
                _logger.LogWarning("Unable to obtain Keycloak token");
                throw new InvalidOperationException("Unable to obtain Keycloak token");
            }

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

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakToken);

            var response = await policy.ExecuteAsync(() => _httpClient.PostAsync(
                $"{sendNotificationUrl}",
                new StringContent(JsonConvert.SerializeObject(httpSendNotificationBody), Encoding.UTF8, "application/json")
            ));

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Send notification failed response: {Response}", responseBody);
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
    public string EventCode { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public IEnumerable<Translation> Translations { get; set; }
}

public interface INotificationSender
{
    Task<bool> SendAsync(NotifyUids notification);
}
