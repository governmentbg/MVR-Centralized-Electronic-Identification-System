using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class AuthorizerUid : UidResult
{
    public Guid Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; }
}
