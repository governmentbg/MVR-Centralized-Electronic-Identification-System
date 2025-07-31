namespace eID.PJS.Contracts.Results;

public class LogUserToMeResult
{
    public string EventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    /// <summary>
    /// It may be Guid
    /// </summary>
    public string RequesterSystemId { get; set; }
    public string RequesterSystemName { get; set; } = string.Empty;
}
