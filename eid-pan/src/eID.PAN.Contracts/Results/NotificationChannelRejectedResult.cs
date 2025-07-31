namespace eID.PAN.Contracts.Results;

public interface NotificationChannelRejectedResult : NotificationChannelResult
{
    string Reason { get; set; }
}
