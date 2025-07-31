using Quartz;

namespace eID.PDEAU.Application.Options;

public class ScrapeIISDAOptions
{
    public string CronTab { get; set; } = "0 0 0 ? * * *"; // Every day in 00:00:00
    public int RetryTimeoutInMinutes { get; set; } = 60;
    public int MaxRetriesCount { get; set; } = 2;

    public void IsValid()
    {
        if (!CronExpression.IsValidExpression(CronTab))
        {
            throw new InvalidOperationException($"Invalid {nameof(ScrapeIISDAOptions)}.{nameof(CronTab)} Cron Expression: '{CronTab}'");
        }
    }
}
