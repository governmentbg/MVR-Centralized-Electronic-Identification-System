namespace eID.PJS.Services;
public interface ICommandStateProvider<T>: IDisposable
{
    bool IsCommandInProgress(string commandType);
    void SetCommandInProgress(string commandType, Guid taskId);
    void RemoveCommandInProgress(string commandType);
    void StoreResult(Guid taskId, T result);
    T? GetResult(Guid taskId);
    void StoreCancellationToken(string commandType, CancellationTokenSource cts);
    CancellationTokenSource? GetCancellationToken(string commandType);
}

