namespace eID.PJS.Contracts.Results.Admin;

public interface LogResult
{
    public string EventId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; }
    /// <summary>
    /// It may be Guid
    /// </summary>
    public string RequesterSystemId { get; set; }
    public string RequesterSystemName { get; set; }
    /// <summary>
    /// Correlation ID to connect the sequence of events together
    /// </summary>
    public string CorrelationId { get; set; }
    /// <summary>
    /// Message to the administartors which describes what was logged.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Gets or sets the requester user identifier.
    /// </summary>
    /// <value>
    /// The requester user identifier.
    /// </value>
    public string RequesterUserId { get; set; }
    /// <summary>
    /// Gets or sets the target user identifier.
    /// </summary>
    /// <value>
    /// The target user identifier.
    /// </value>
    public string TargetUserId { get; set; }
    /// <summary>
    /// Event payload must be JSON serializable. 
    public Dictionary<string, object> EventPayload { get; set; }
}
