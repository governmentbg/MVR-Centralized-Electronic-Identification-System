using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;
using eID.PJS.Services.AsyncExclusiveCommand;

namespace eID.PJS.Services;
public class InMemoryCommandStateProvider<T> : ICommandStateProvider<T>
{
    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly InMemoryCommandStateProviderSettings _settings;
    private readonly object _lock = new object();

    public InMemoryCommandStateProvider(InMemoryCommandStateProviderSettings settings)
    {
        _settings = settings;
    }

    public bool IsCommandInProgress(string commandType)
    {
        lock (_lock)
        {
            return _cache.TryGetValue(commandType, out _);
        }
    }

    public void SetCommandInProgress(string commandType, Guid taskId)
    {
        lock (_lock)
        {
            _cache.Set(commandType, taskId, _settings.CommandStatusTTL);
        }
    }

    public void RemoveCommandInProgress(string commandType)
    {
        lock (_lock)
        {
            _cache.Remove(commandType);
        }
    }

    public void StoreResult(Guid taskId, T result)
    {
        lock (_lock)
        {
            _cache.Set(taskId.ToString(), result, _settings.ResultTTL); 

            _cache.TryGetValue(taskId.ToString(), out T? data);

            if (data == null)
                throw new InvalidOperationException("Failed to store value in the cache");
        }
    }

    public T? GetResult(Guid taskId)
    {
        lock (_lock)
        {
            _cache.TryGetValue(taskId.ToString(), out T? result);
            return result;
        }
    }

    public void StoreCancellationToken(string commandType, CancellationTokenSource cts)
    {
        lock (_lock)
        {
            _cache.Set($"{commandType}_cts", cts, _settings.CommandStatusTTL);
        }
    }

    public CancellationTokenSource? GetCancellationToken(string commandType)
    {
        lock (_lock)
        {
            _cache.TryGetValue($"{commandType}_cts", out CancellationTokenSource? cts);
            return cts;
        }
    }

    public void Dispose()
    {
        if (_cache != null)
            _cache.Dispose();
    }
}
