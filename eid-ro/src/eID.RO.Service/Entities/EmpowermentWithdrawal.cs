using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class EmpowermentWithdrawal : EmpowermentWithdrawResult
{
    public Guid Id { get; set; }
    /// <summary>
    /// Withdrawal process initiation datetime (UTC)
    /// </summary>
    public DateTime StartDateTime { get; set; }
    /// <summary>
    /// On this date withdrawal was confirmed by all authorizers and became active. (UTC)
    /// </summary>
    public DateTime? ActiveDateTime { get; set; }
    /// <summary>
    /// Person who initiated the withdrawal process
    /// </summary>
    [EncryptProperty]
    public string IssuerUid { get; set; } = string.Empty;
    /// <summary>
    /// IssuerUid type
    /// </summary>
    public IdentifierType IssuerUidType { get; set; }
    /// <summary>
    /// Withdraw reason
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>
    /// Withdraw empowerment status
    /// </summary>
    public EmpowermentWithdrawalStatus Status { get; set; }
    /// <summary>
    /// Base64 encoded response from timestamping server.
    /// </summary>
    public string TimestampData { get; set; } = string.Empty;

    /// <summary>
    /// EmpowermentStatement FK
    /// </summary>
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; } = new EmpowermentStatement();
    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }

    public override string ToString() => string.Join(",", Id, EmpowermentStatementId, StartDateTime, IssuerUid, IssuerUidType, Reason);
}
