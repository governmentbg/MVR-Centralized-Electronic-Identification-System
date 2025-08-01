using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace eID.PDEAU.Service;

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

    public static IAsyncPolicy<HttpResponseMessage> GetCallRegiXPolicy(ILogger logger, string registerName)
    {
        return Policy<HttpResponseMessage>
            .Handle<Exception>()
            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
            .WaitAndRetryAsync(3,
                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                (exception, timespan) =>
                {
                    logger.LogInformation("Failed getting actual state from {registerName}. Next attempt will be at {NextAttemptTime}", registerName, DateTime.UtcNow.Add(timespan));
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetPANCallPolicy(ILogger logger)
    {
        var statusesWeWontRetry = new HttpStatusCode[] {
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Forbidden,
                HttpStatusCode.NotFound,
                HttpStatusCode.Conflict
            };

        var policy = Policy<HttpResponseMessage>
                        .Handle<Exception>()
                        .OrResult(response =>
                                    !response.IsSuccessStatusCode
                                        && !statusesWeWontRetry.Contains(response.StatusCode)
                                        // HandleTransientHttpError
                                        && response.StatusCode >= HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.RequestTimeout)
                        .WaitAndRetryForeverAsync(
                            _ => TimeSpan.FromSeconds(60),
                            (exception, timespan) =>
                            {
                                logger.LogWarning(
                                    exception.Exception,
                                    "Failed event registration. StatusCode: ({StatusCode}) {Response}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode, exception.Result?.ToString(), DateTime.UtcNow.Add(timespan));
                            });

        return policy;
    }
}
