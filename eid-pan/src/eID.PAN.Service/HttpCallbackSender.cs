using System.Net;
using System.Text;
using eID.PAN.Contracts.Results;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.PAN.Service;

public class HttpCallbackSender : IHttpCallbackSender
{
    private readonly ILogger<HttpCallbackSender> _logger;
    private readonly HttpClient _httpClient;
    private readonly HttpClient _rawHttpClient;

    public HttpCallbackSender(ILogger<HttpCallbackSender> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient(ApplicationPolicyRegistry.HttpClientWithRetryPolicy);
        _rawHttpClient = httpClientFactory.CreateClient();
    }

    public async Task<bool> SendHttpCallbackAsync(string callbackUrl, object body)
    {
        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new ArgumentException($"'{nameof(callbackUrl)}' cannot be null or whitespace.", nameof(callbackUrl));
        }

        if (body is null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        try
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(callbackUrl),
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            });

            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully sent callback to {Url}", callbackUrl);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error occurred when trying to send callback request to {Url}", callbackUrl);
            return false;
        }
    }

    public async Task<TestHttpCallbackResult> TestHttpCallbackAsync(string callbackUrl)
    {
        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new ArgumentException($"'{nameof(callbackUrl)}' cannot be null or whitespace.", nameof(callbackUrl));
        }

        try
        {
            var response = await _rawHttpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(callbackUrl),
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    UserId = Guid.NewGuid(),
                    Translations = new List<object>
            {
                new  { Language = "en", Message = "This is a test message for connectivity check from the automatic notifications subsystem of the centralized electronic identification system (CEIS)!" },
                new  { Language = "bg", Message = "Това е тестово съобщение за проверка на свързаност, от подсистемата за автоматични нотификации на централизираната система за електронна идентификация (ЦСЕИ)!" }
            }
                }), Encoding.UTF8, "application/json")
            });

            _logger.LogInformation("Sent test callback to {Url}. Received: {StatusCode}", callbackUrl, response.StatusCode);
            return new TestHttpCallbackResultDTO
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Response = response.IsSuccessStatusCode ? string.Empty : await response.Content.ReadAsStringAsync()
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error occurred when trying to send test callback request to {Url}", callbackUrl);
            return new TestHttpCallbackResultDTO
            {
                IsSuccess = false,
                StatusCode = ex.StatusCode,
                Response = ex.Message
            };
        }
    }
}

public interface IHttpCallbackSender
{
    Task<bool> SendHttpCallbackAsync(string callbackUrl, object body);
    Task<TestHttpCallbackResult> TestHttpCallbackAsync(string callbackUrl);
}

public class TestHttpCallbackResultDTO : TestHttpCallbackResult
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Response { get; set; }
}
