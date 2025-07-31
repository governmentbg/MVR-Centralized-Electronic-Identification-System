namespace eID.PIVR.Service;

public class ApiUsageTrackerServiceBase
{
    protected readonly string _cacheKeyPrefix = "eID:PIVR:api-usage";

    protected string GetCacheKey(string registryKey, DateTime date)
    => $"{_cacheKeyPrefix}:{registryKey}:{date:yyyy-MM-dd}";
}
