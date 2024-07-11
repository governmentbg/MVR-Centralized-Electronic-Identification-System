using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

/// <summary>
/// Describe withdraw action
/// </summary>
public interface EmpowermentWithdrawResult
{
    /// <summary>
    /// DateTime when withdrawal process was initiated (UTC)
    /// </summary>
    DateTime StartDateTime { get; }
    /// <summary>
    /// DateTime when all authorizers confirmed the withdrawal (UTC)
    /// </summary>
    DateTime? ActiveDateTime { get; }
    /// <summary>
    /// Who initiated the withdrawal process
    /// </summary>
    string IssuerUid { get; }
    /// <summary>
    /// Issuer type
    /// </summary>
    IdentifierType IssuerUidType { get; }
    /// <summary>
    /// Reason for withdrawal
    /// </summary>
    string Reason { get; }
    /// <summary>
    /// Current withdrawal status
    /// </summary>
    EmpowermentWithdrawalStatus Status { get; }
}
