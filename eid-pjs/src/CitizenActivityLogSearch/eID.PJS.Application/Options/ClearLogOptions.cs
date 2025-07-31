using Quartz;

namespace eID.PJS.Application.Options;

public class ClearLogOptions
{
    public string CronTab { get; set; } = "0 0 0 ? * * *"; // Every day in 00:00:00
    public int RemoveOlderFilesInDays { get; set; } = 5;

    public void IsValid()
    {
        if (!CronExpression.IsValidExpression(CronTab))
        {
            throw new InvalidOperationException($"Invalid {nameof(ClearLogOptions)}.{nameof(CronTab)} Cron Expression: '{CronTab}'");
        }
    }
}
