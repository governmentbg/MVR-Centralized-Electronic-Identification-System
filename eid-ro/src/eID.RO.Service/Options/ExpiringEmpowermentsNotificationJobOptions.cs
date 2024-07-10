namespace eID.RO.Service.Options;

public class ExpiringEmpowermentsNotificationJobOptions
{
    public string CronPeriod { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CronPeriod))
        {
            throw new ArgumentNullException(nameof(CronPeriod));
        }
    }
}
