namespace eID.PJS.Contracts.Results.Admin;

public class LogFromUserResult : LogResult
{
    public string EventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; }
    public string RequesterSystemId { get; set; }
    public string RequesterSystemName { get; set; }
    public string CorrelationId { get; set; }
    public string Message { get; set; }
    public string RequesterUserId { get; set; }
    public string TargetUserId { get; set; }
    public Dictionary<string, object> EventPayload { get; set; }
    public string SystemId { get; set; }
    public string ModuleId { get; set; }
}
