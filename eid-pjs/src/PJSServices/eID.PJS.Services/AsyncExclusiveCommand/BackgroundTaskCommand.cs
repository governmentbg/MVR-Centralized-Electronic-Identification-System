using System.Data;

using OpenSearch.Client;

namespace eID.PJS.Services;
/// <summary>Abstract base class for the background command run exclusively </summary>
/// <typeparam name="T"></typeparam>
public abstract class BackgroundTaskCommand<T>
{
    private readonly ICommandStateProvider<T> _stateProvider;
    private readonly string _commandType;
    private readonly ILogger _logger;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public event EventHandler<TaskFinishedEventArgs<T>>? TaskFinished;

    public BackgroundTaskCommand(ICommandStateProvider<T> stateProvider, ILogger logger)
    {
        _commandType = GetType().FullName ?? GetType().Name;
        _stateProvider = stateProvider ?? throw new ArgumentException(nameof(stateProvider));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
    /// <returns></returns>
    public async Task<CommandResult> ExecuteAsync(int? timeoutInMilliseconds = null)
    {
        // Wait till the command has started or found that other command is already in progress.
        await _semaphore.WaitAsync();
        var taskId = Guid.NewGuid();

        try
        {
            // If there is already a command of the same type running then exit.
            if (_stateProvider.IsCommandInProgress(_commandType))
            {
                _logger.LogDebug("Cammand in progress for command type '{ct}'.", _commandType);
                return new CommandResult(Guid.Empty, null, CommandResultType.TaskAlreadyInProgress);
            }
          
            _logger.LogDebug("SetCommandInProgress for command type '{ct}' and task id '{tid}'", _commandType, taskId);
            _stateProvider.SetCommandInProgress(_commandType, taskId);

            // If a timeout value is provided, the token will automatically request cancellation after the specified timeout. 
            CancellationTokenSource cts = timeoutInMilliseconds.HasValue ? new CancellationTokenSource(timeoutInMilliseconds.Value) : new CancellationTokenSource();

            if (timeoutInMilliseconds.HasValue)
            {
                _stateProvider.StoreCancellationToken(_commandType, cts);
                _logger.LogDebug("StoreCancellationToken for command type '{ct}'.", _commandType);
            }

            // Kind of fire and forget. 
            // Schedule the task to be executed in a background thread.
            // We're intentionally not awaiting this task here so we'll know when it finishes by catching the TaskFinished event
            _ = Task.Run(() =>
            {
                try
                {
                    var result = ExecuteTask(cts.Token);
                    if (!cts.Token.IsCancellationRequested)
                    {
                        _logger.LogDebug("Storing result for command type '{ct}' and task id '{tid}'", _commandType, taskId);
                        _stateProvider.StoreResult(taskId, result);
                        OnTaskFinished(new TaskFinishedEventArgs<T>(taskId, result));
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("OperationCanceledException for command type '{ct}' and task id '{tid}'", _commandType, taskId);
                    OnTaskFinished(new TaskFinishedEventArgs<T>(taskId, default, new TimeoutException("Task was cancelled due to timeout.")));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception executing command type '{ct}' and task id '{tid}'", _commandType, taskId);
                    OnTaskFinished(new TaskFinishedEventArgs<T>(taskId, default, ex));
                }
                finally
                {
                    _stateProvider.RemoveCommandInProgress(_commandType);
                    _stateProvider.GetCancellationToken(_commandType)?.Dispose();
                }
            });

            return new CommandResult(taskId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception executing command type '{ct}'.", _commandType);
            return new CommandResult(taskId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected abstract T ExecuteTask(CancellationToken cancellationToken);

    protected virtual void OnTaskFinished(TaskFinishedEventArgs<T> e)
    {
        TaskFinished?.Invoke(this, e);
    }
}
