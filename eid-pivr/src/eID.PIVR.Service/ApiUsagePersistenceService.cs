using eID.PIVR.Service.Database;
using eID.PIVR.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace eID.PIVR.Service;

public class ApiUsagePersistenceService : ApiUsageTrackerServiceBase, IApiUsagePersistenceService
{
    private readonly IDatabase _redisDb;
    private readonly ILogger<ApiUsageTrackerService> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly ApplicationDbContext _dbContext;

    public ApiUsagePersistenceService(
        IConnectionMultiplexer redis,
        ILogger<ApiUsageTrackerService> logger,
        ApplicationDbContext dbContext)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _redisDb = redis.GetDatabase();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task FlushToDatabaseAsync()
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var today = DateTime.UtcNow.Date;

            var pattern = $"{_cacheKeyPrefix}:*";

            var keys = new List<RedisKey>();
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                var parts = key.ToString().Split(':');
                if (parts.Length != 5)
                {
                    _logger.LogWarning("Detected API statistics key with unexpected format. Key: {Key}", key.ToString());
                    continue;
                }

                string datePart = parts[4];
                if (DateTime.TryParse(datePart, out var keyDate))
                {
                    if (keyDate < today) // Takes keys older than today
                    {
                        keys.Add(key);
                    }
                }
            }

            foreach (var key in keys)
            {
                try
                {
                    var value = await _redisDb.StringGetAsync(key);
                    if (!value.HasValue)
                    {
                        _logger.LogInformation("Skipping API statistics key with no value. Key: {Key}", key.ToString());
                        continue;
                    }

                    var parts = key.ToString().Split(':');
                    if (!DateOnly.TryParseExact(parts[4], "yyyy-MM-dd", out DateOnly date))
                    {
                        _logger.LogWarning("Detected API statistics key with unexpected datetime format. Key: {Key}; Date: {Date}", key.ToString(), parts[4]);
                        continue;
                    }

                    string registryKey = parts[3];
                    int count = (int)value;
                    _dbContext.ApiUsageStats.Add(new ApiUsageStat
                    {
                        RegistryKey = registryKey,
                        Date = date,
                        Count = count
                    });
                    // Intentionally deleting redis cache before storing the data in the database.
                    // We're more comfortable with the idea to lose a day worth of call information instead of filling the cache with poisonous messages.
                    await _redisDb.KeyDeleteAsync(key);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Failed to save to {RedisKeyValue} database", key.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception during flushing {RedisKeyValue}", key.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to flush API usage to database");
        }
    }
}

public interface IApiUsagePersistenceService
{
    Task FlushToDatabaseAsync();
}
