using MassTransit;

namespace eID.PJS.Contracts.Commands;

public interface GetLogDeau : CorrelatedBy<Guid>
{
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
    /// System Id
    /// </summary>
    string SystemId { get; }
    
    /// <summary>
    /// Size of cursor return data
    /// </summary>
    int CursorSize { get; }
    
    /// <summary>
    /// The parameter provides a live cursor that uses the previous page’s results to obtain the next page’s results
    /// In the first request it will be <see cref="null"/>
    /// </summary>
    IEnumerable<object> CursorSearchAfter { get; }
}
