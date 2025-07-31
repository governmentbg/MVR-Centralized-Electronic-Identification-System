using System;

namespace eID.PJS.Services;

public interface IBackgroundTaskCommand<T>
{
    event EventHandler<TaskFinishedEventArgs<T>>? TaskFinished;
    CommandResult Execute(int? timeoutInMilliseconds = null);
    string CommandType { get; }
}
