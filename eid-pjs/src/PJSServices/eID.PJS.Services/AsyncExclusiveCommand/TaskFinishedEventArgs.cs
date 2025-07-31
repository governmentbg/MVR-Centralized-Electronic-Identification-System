namespace eID.PJS.Services;
public class TaskFinishedEventArgs<T> : EventArgs
{
    public Guid TaskId { get; }
    public T? Result { get; }
    public Exception? Error { get; }

    public TaskFinishedEventArgs(Guid taskId, T? result, Exception? error = null)
    {
        TaskId = taskId;
        Result = result;
        Error = error;
    }
}
