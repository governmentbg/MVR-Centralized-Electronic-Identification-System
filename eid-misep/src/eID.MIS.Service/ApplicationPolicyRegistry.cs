using System.Net;
using Microsoft.Extensions.Logging;
using Polly;

namespace eID.MIS.Service;

public class ApplicationPolicyRegistry
{
    public const string HttpClientWithRetryPolicy = "retryHttpClientPolicy";

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return
            Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            // Custom transient handling policy
            .OrResult(r => (int)r.StatusCode > 500 // Excluding 500 since integrations service seldom recovers from 500
                            || r.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryCount, response, context) => TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 100),
                onRetryAsync: async (response, timespan, retryCount, context) =>
                {
                    logger.LogWarning(
                            "ApplicationPolicyRegistry: ({IntStatusCode}) {StatusCode} occurred. " +
                            "Waiting for: {Timespan}. Retry count: {RetryCount}",
                            (int?)response.Result?.StatusCode,
                            response.Result?.StatusCode,
                            timespan,
                            retryCount
                        );
                    await Task.CompletedTask;
                }
            );
    }
}
