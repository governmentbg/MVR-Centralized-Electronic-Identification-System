using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace eID.RO.Service.Extensions;

public static class DistributedCacheExtensions
{
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpireTime = null)
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            SlidingExpiration = slidingExpireTime,
        };

        return SetAsync(cache, key, value, options);
    }

    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var data = JsonConvert.SerializeObject(value);
        await cache.SetStringAsync(key, data, options);
    }

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        var cacheData = await cache.GetStringAsync(key);

        if (cacheData == null)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(cacheData);
    }

    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        var cacheData = cache.GetString(key);
        if (cacheData == null)
        {
            value = default;
            return false;
        }

        value = JsonConvert.DeserializeObject<T>(cacheData);
        return true;
    }

    public static async Task<T?> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> factory)
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (!cache.TryGetValue(key, out T? result))
        {
            result = await factory().ConfigureAwait(false);
        }

        return result;
    }
}
