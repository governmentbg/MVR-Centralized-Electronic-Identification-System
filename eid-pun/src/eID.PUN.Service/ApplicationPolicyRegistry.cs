using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace eID.PUN.Service;

public class ApplicationPolicyRegistry
{
    public const string HttpClientWithRetryPolicy = "retryHttpClientPolicy";

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryCount, response, context) =>
                {
                    var delta = response?.Result?.Headers?.RetryAfter?.Delta ?? TimeSpan.Zero;
                    if (TimeSpan.Zero != delta)
                    {
                        return delta;
                    }

                    return TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                },
                onRetryAsync: async (response, timespan, retryCount, context) =>
                {
                    var message = $"ApplicationPolicyRegistry: ({(int?)response.Result?.StatusCode}) {response.Result?.StatusCode} occurred. " +
                            $"Waiting for: {timespan}. Retry count: {retryCount}";

                    if (response.Result?.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        logger.LogWarning(message);
                    }
                    else
                    {
                        logger.LogInformation(message);
                    }

                    await Task.CompletedTask;
                }
            );
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRapidRetryPolicy(ILogger logger)
    {
        return
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryCount, response, context) =>
                {
                    var delta = response?.Result?.Headers?.RetryAfter?.Delta ?? TimeSpan.Zero;
                    if (TimeSpan.Zero != delta)
                    {
                        return delta;
                    }

                    return TimeSpan.FromMilliseconds(100);
                },
                onRetryAsync: async (response, timespan, retryCount, context) =>
                {
                    var message = $"ApplicationPolicyRegistry: ({(int?)response.Result?.StatusCode}) {response.Result?.StatusCode} occurred. " +
                            $"Waiting for: {timespan}. Retry count: {retryCount}";

                    if (response.Result?.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        logger.LogWarning(message);
                    }
                    else
                    {
                        logger.LogInformation(message);
                    }

                    await Task.CompletedTask;
                }
            );
    }
}
