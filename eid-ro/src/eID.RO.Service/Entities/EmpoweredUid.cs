using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class EmpoweredUid : UidResult
{
    public Guid Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }

    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; }
}
