
namespace eID.PJS.Services;
public class SampleCommand : BackgroundTaskCommand<string>
{
    public SampleCommand(ICommandStateProvider<string> stateProvider, ILogger logger) : base(stateProvider, logger) { }

    protected override string ExecuteTask(CancellationToken cancellationToken)
    {
        // Simulate some work with cancellation support
        for (int i = 0; i < 10; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Task.Delay(1000).Wait(); // Simulate 10 seconds of work
        }

        return  "Result: Task Finished!";
    }
}
