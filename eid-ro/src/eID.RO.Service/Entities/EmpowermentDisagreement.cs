using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class EmpowermentDisagreement : EmpowermentDisagreementResult
{
    public Guid Id { get; set; }
    /// <summary>
    /// On this date disagreement became active. (UTC)
    /// </summary>
    public DateTime ActiveDateTime { get; set; }
    /// <summary>
    /// Person who initiated the disagreement process
    /// </summary>
    [EncryptProperty]
    public string IssuerUid { get; set; } = string.Empty;
    
    /// <summary>
    /// Person Uid type
    /// </summary>
    public IdentifierType IssuerUidType { get; set; }

    /// <summary>
    /// Disagreement reason
    /// </summary>
    public string Reason { get; set; } = string.Empty;

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
    public override string ToString() => string.Join(",", Id, EmpowermentStatementId, ActiveDateTime, IssuerUid, IssuerUidType, Reason);
}
