using MassTransit;

namespace eID.PJS.Contracts.Commands;

public interface GetLogUserFromMe : CorrelatedBy<Guid>
{
    /// <summary>
    /// User id got from token
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// User Uid from token
    /// </summary>
    string UserUid { get; }

    /// <summary>
    /// User Uid type from token
    /// </summary>
    IdentifierType UserUidType { get; }

    /// <summary>
    /// Filter for start date. Optional.
    /// </summary>
    DateTime? StartDate { get; }

    /// <summary>
    /// Filter for end date. Optional.
    /// </summary>
    DateTime? EndDate { get; }

    /// <summary>
    /// Type or events to be obtained. Optional
    /// </summary>
    string[]? EventTypes { get; }

    /// <summary>
    /// Excluded event types. Optional
    /// </summary>
    string[]? ExcludedEventTypes { get; }

    /// <summary>
    /// Size of cursor return data
    /// </summary>
    int CursorSize { get; }
    
    /// <summary>
    /// The parameter provides a live cursor that uses the previous page’s results to obtain the next page’s results
    /// In the first request it will be <see cref="null"/>
    /// </summary>
    IEnumerable<object>? CursorSearchAfter { get; }
}
