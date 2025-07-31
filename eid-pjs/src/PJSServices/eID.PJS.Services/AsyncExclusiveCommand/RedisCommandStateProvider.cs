using eID.PJS.Services.AsyncExclusiveCommand;

using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace eID.PJS.Services;
public class RedisCommandStateProvider<T> : ICommandStateProvider<T>
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new();
    private readonly RedisCommandStateProviderSettings _settings;

    public RedisCommandStateProvider(RedisCommandStateProviderSettings settings)
    { 
        _settings = settings;
        _redis = ConnectionMultiplexer.Connect(_settings.ConnectionString);
        _db = _redis.GetDatabase();
    }
    public bool IsCommandInProgress(string commandType)
    {
        return _db.KeyExists(commandType);
    }

    public void SetCommandInProgress(string commandType, Guid taskId)
    {
        _db.StringSet(commandType, taskId.ToString(), _settings.CommandStatusTTL);
    }

    public void RemoveCommandInProgress(string commandType)
    {
        _db.KeyDelete(commandType);
    }

    public void StoreResult(Guid taskId, T result)
    {
        var jsonResult = JsonConvert.SerializeObject(result);
        _db.StringSet(taskId.ToString(), jsonResult, _settings.ResultTTL);
    }

    public T? GetResult(Guid taskId)
    {
        var result = _db.StringGet(taskId.ToString());
        
        if (result.HasValue)
            return JsonConvert.DeserializeObject<T>(result.ToString());

        return default(T?);
    }

    public void StoreCancellationToken(string commandType, CancellationTokenSource cts)
    {
        _cancellationTokens[commandType] = cts;
    }

    public CancellationTokenSource? GetCancellationToken(string commandType)
    {
        _cancellationTokens.TryGetValue(commandType, out var cts);
        return cts;
    }

    public void Dispose()
    {
        if (_redis != null)
            _redis.Dispose();
       
    }
}
