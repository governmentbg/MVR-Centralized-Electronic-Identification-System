using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace eID.PJS.Services.Tests;

public class AsyncExclusiveCommandTests
{

    public AsyncExclusiveCommandTests()
    {

    }

    [Fact]
    public void SampleCommandTestAsync()
    {
        ICommandStateProvider<string> stateProvider = new InMemoryCommandStateProvider<string>(new InMemoryCommandStateProviderSettings
        {
            ResultTTL = TimeSpan.FromMinutes(10),
            CommandStatusTTL = TimeSpan.FromMinutes(10)
        });

        var command = new SampleCommand(stateProvider, NullLogger.Instance);

        command.TaskFinished += (sender, e) =>
        {
            if (e.Error != null)
            {
                Debug.WriteLine($"Task {e.TaskId} encountered an error: {e.Error.Message}");
            }
            else
            {
                Debug.WriteLine($"Task {e.TaskId} finished with result: {e.Result}");
            }
        };

        Parallel.For(0, 10, async i =>
        {
            var result = await command.ExecuteAsync(); // Using timeout for demonstration
            HandleCommandResult(result);
        });

    }

    private static void HandleCommandResult(CommandResult result)
    {
        if (result.ResultType == CommandResultType.TaskAlreadyInProgress)
        {
            Debug.WriteLine("Task is already in progress.");
        }
        else if (result.ResultType == CommandResultType.Error)
        {
            Debug.WriteLine($"Error starting task: {result.Error?.Message}");
        }
        else
        {
            Debug.WriteLine($"Started task with ID: {result.TaskId}");
        }
    }
}

