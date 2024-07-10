namespace eID.RO.Contracts.Enums;

/// <summary>
/// Withdrawal status
/// </summary>
public enum EmpowermentWithdrawalStatus
{
    None = 0,
    /// <summary>
    /// Withdrawal process is ongoing
    /// </summary>
    InProgress = 1,
    /// <summary>
    /// Process was completed successfully
    /// </summary>
    Completed = 2,
    /// <summary>
    /// Checks were not successful
    /// </summary>
    Denied = 3,
    /// <summary>
    /// Withdrawal confirmations were not collected on time
    /// </summary>
    Timeout = 4,
}
