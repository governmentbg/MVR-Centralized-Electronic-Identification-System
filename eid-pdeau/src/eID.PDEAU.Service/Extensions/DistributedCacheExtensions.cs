using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Extensions;

public static class DistributedCacheExtensions
{
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
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
            SlidingExpiration = slidingExpiration,
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

    public static async Task<T?> GetOrAddAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> fetchData,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
        where T : class
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        if (fetchData is null)
        {
            throw new ArgumentNullException(nameof(fetchData));
        }

        if (!cache.TryGetValue(key, out T? result))
        {
            result = await fetchData().ConfigureAwait(false);

            if (result is not null)
            {
                await cache.SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration);
            }
        }

        return result;
    }

    public static async Task<T> GetOrAddDefAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> fetchData,
        T defaultValue,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
        where T : class
    {
        if (cache is null)
        {
            throw new ArgumentNullException(nameof(cache));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        if (fetchData is null)
        {
            throw new ArgumentNullException(nameof(fetchData));
        }

        if (defaultValue is null)
        {
            throw new ArgumentNullException(nameof(defaultValue));
        }

        if (!cache.TryGetValue(key, out T? result))
        {
            result = await fetchData().ConfigureAwait(false) ?? defaultValue;
            await cache.SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration);
        }

        return result ?? defaultValue;
    }
}
