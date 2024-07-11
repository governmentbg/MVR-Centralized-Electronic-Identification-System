using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.PAN.Service;

public class HttpCallbackSender : IHttpCallbackSender
{
    private readonly ILogger<HttpCallbackSender> _logger;
    private readonly HttpClient _httpClient;

    public HttpCallbackSender(ILogger<HttpCallbackSender> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient(ApplicationPolicyRegistry.HttpClientWithRetryPolicy);
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
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error occurred when trying to send callback request to {Url}", callbackUrl);
            return false;
        }

        _logger.LogInformation("Successfully sent callback to {Url}", callbackUrl);
        return true;
    }
}

public interface IHttpCallbackSender
{
    Task<bool> SendHttpCallbackAsync(string callbackUrl, object body);
}
