namespace eID.PJS.Contracts.Results;

public class LogUserFromMeResult
{
    public string EventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
}
