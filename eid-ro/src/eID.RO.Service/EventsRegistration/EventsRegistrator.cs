using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace eID.RO.Service.EventsRegistration;

public class EventsRegistrator : IEventsRegistrator
{
    private readonly ILogger<EventsRegistrator> _logger;
    private readonly HttpClient _httpClient;
    private readonly IKeycloakCaller _keycloakCaller;

    public EventsRegistrator(
        ILogger<EventsRegistrator> logger,
        IHttpClientFactory httpClientFactory,
        IKeycloakCaller keycloakCaller)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
        _keycloakCaller = keycloakCaller ?? throw new ArgumentNullException(nameof(keycloakCaller));
    }

    public async Task<bool> RegisterAsync()
    {
        var registrationUrl = "/api/v1/notifications/systems";
        var httpRegistrationBody = BuildRegistrationHttpBody();

        _logger.LogInformation("Attempting events registration...");

        try
        {
            var keycloakToken = await _keycloakCaller.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(keycloakToken))
            {
                _logger.LogWarning("Unable to obtain Keycloak token");
                throw new InvalidOperationException("Unable to obtain Keycloak token");
            }

            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
                            .WaitAndRetryForeverAsync(
                            _ => TimeSpan.FromSeconds(60),
                            (exception, timespan) =>
                            {
                                _logger.LogInformation("Failed event registration. Next attempt will be at {NextAttemptTime}", DateTime.UtcNow.Add(timespan));
                            });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakToken);

            var response = await policy.ExecuteAsync(() => _httpClient.PostAsync(
                registrationUrl,
                new StringContent(JsonConvert.SerializeObject(httpRegistrationBody), Encoding.UTF8, "application/json")
            ));
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Event registration failed response: {Response}", responseBody);
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to register. Request: POST {Url}, Body {Body}", registrationUrl, httpRegistrationBody);
            return false;
        }

        _logger.LogInformation("Event registration request completed successfully");
        return true;
    }

    private static object BuildRegistrationHttpBody()
    {
        var body = new
        {
            Translations = new List<object>
            {
                new { Language = "bg", Name = "Регистър на овластяванията" },
                new { Language = "en", Name = "Authorizations register" }
            },
            Events = Events.GetAllEvents()
        };

        return body;
    }
}

public interface IEventsRegistrator
{
    Task<bool> RegisterAsync();
}
