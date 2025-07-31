using MassTransit;

namespace eID.PJS.Contracts.Commands.Admin;

public interface GetLogFromUserAsFile : CorrelatedBy<Guid>
{
    /// <summary>
    /// User id
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// User eidentity id taken from token
    /// </summary>
    string UserEid { get; }

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
