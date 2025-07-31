namespace eID.PJS.Contracts.Results;

public class LogDeauResult
{
    public Guid EventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
}
