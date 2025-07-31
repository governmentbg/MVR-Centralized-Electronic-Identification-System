using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace eID.RO.Service.EventsRegistration;

public class EventsRegistrator : IEventsRegistrator
{
    private readonly ILogger<EventsRegistrator> _logger;
    private readonly HttpClient _httpClient;

    public EventsRegistrator(
        ILogger<EventsRegistrator> logger,
        IHttpClientFactory httpClientFactory)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
    }

    public async Task<bool> RegisterAsync()
    {
        var registrationUrl = "/api/v1/notifications/systems";
        var httpRegistrationBody = BuildRegistrationHttpBody();

        _logger.LogInformation("Attempting events registration...");

        try
        {
            var statusesWeWontRetry = new System.Net.HttpStatusCode[] {
                System.Net.HttpStatusCode.BadRequest,
                System.Net.HttpStatusCode.NotFound,
                System.Net.HttpStatusCode.Unauthorized,
                System.Net.HttpStatusCode.Forbidden
            };
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode
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
                registrationUrl,
                new StringContent(JsonConvert.SerializeObject(httpRegistrationBody), Encoding.UTF8, MediaTypeNames.Application.Json)
            ));
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Event registration failed response: ({StatusCode}) {Response}", response.StatusCode, response.ToString());
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

    private object BuildRegistrationHttpBody()
    {
        var body = new
        {
            Translations = new List<object>
            {
                new { Language = "bg", Name = "Регистър на овластяванията" },
                new { Language = "en", Name = "Authorizations register" }
            },
            Events = Events.GetAllEvents(_logger)
        };

        return body;
    }
}

public interface IEventsRegistrator
{
    Task<bool> RegisterAsync();
}
