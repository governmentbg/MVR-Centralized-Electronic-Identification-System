using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

/// <summary>
/// Empowerment statement history status
/// </summary>
public interface StatusHistoryResult
{
    /// <summary>
    /// Status Id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// DateTime on which the empowerment statement got this status (UTC)
    /// </summary>
    public DateTime DateTime { get; set; }
    /// <summary>
    /// Status of the empowerment statement
    /// </summary>
    public EmpowermentStatementStatus Status { get; set; }
}
