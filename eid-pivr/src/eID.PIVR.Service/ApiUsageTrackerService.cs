using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace eID.PIVR.Service;

public class ApiUsageTrackerService : ApiUsageTrackerServiceBase, IApiUsageTrackerService
{
    private readonly ILogger<ApiUsageTrackerService> _logger;
    private readonly IDatabase _redisDb;

    public ApiUsageTrackerService(
        IConnectionMultiplexer redis,
        ILogger<ApiUsageTrackerService> logger)
    {
        if (redis is null)
        {
            throw new ArgumentNullException(nameof(redis));
        }

        _redisDb = redis.GetDatabase();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task TrackUsageAsync(string registryKey)
    {
        try
        {
            string redisKey = GetCacheKey(registryKey, DateTime.UtcNow);
            await _redisDb.StringIncrementAsync(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track API usage for key {RegistryKey}", registryKey);
        }
    }
}

public interface IApiUsageTrackerService
{
    Task TrackUsageAsync(string registryKey);
}
