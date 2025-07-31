using System;
using System.Threading;
using System.Threading.Tasks;

namespace eID.PJS.Services;
public class CommandResult
{
    public Guid TaskId { get; }
    public Exception? Error { get; }
    public CommandResultType ResultType { get; }

    public CommandResult(Guid taskId, Exception? error = null, CommandResultType errorType = CommandResultType.Started)
    {
        TaskId = taskId;
        Error = error;
        ResultType = errorType;
    }
}
