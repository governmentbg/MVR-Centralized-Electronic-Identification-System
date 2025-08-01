#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Entities;

public class AdministratorAction : AdministratorActionResult
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string AdministratorUid { get; set; }
    public IdentifierType AdministratorUidType { get; set; }
    public string AdministratorFullName { get; set; }
    public AdministratorActionType Action { get; set; }
    public string Comment { get; set; }

    // User
    [JsonIgnore]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; }
}
#nullable restore
