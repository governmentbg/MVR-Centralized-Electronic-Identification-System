using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace eID.PDEAU.API.Public;

public class ApplicationPolicyRegistry
{
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
}

