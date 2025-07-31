using MassTransit;

namespace eID.PJS.Contracts.Commands.Admin;

public interface GetLogToUserAsFile : CorrelatedBy<Guid>
{
    /// <summary>
    /// User id got from token
    /// </summary>
    string UserId { get; }
    string RequesterUid { get; }
    IdentifierType RequesterUidType { get; }
    string TargetUid { get; }
    IdentifierType TargetUidType { get; }
    string TargetName { get; }

    /// <summary>
    /// Filter for start date. Optional.
    /// </summary>
    DateTime StartDate { get; }

    /// <summary>
    /// Filter for end date. Optional.
    /// </summary>
    DateTime EndDate { get; }

    /// <summary>
    /// Type or events to be obtained. Optional
    /// </summary>
    string[]? EventTypes { get; }
    
    string EventId { get; }
    
    public string FileFullPath { get; set; }
}
