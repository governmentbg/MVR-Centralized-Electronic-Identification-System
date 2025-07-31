using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class EmpowermentSignature : EmpowermentSignatureResult
{
    public Guid Id { get; set; }
    /// <summary>
    /// Signing datetime (UTC)
    /// </summary>
    public DateTime DateTime { get; set; }
    /// <summary>
    /// Person who's signature is being stored
    /// </summary>
    [EncryptProperty]
    public string SignerUid { get; set; } = string.Empty;

    /// <summary>
    /// SignerUid type
    /// </summary>
    public IdentifierType SignerUidType { get; set; }

    public string Signature { get; set; }

    /// <summary>
    /// EmpowermentStatement FK
    /// </summary>
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; } = new EmpowermentStatement();
    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }
}
