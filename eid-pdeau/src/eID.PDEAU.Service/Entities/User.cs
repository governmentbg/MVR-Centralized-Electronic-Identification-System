#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Entities;

public class User : UserResult
{
    public Guid Id { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    /// <summary>
    /// Electronic identity Id
    /// </summary>
    public string EID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsAdministrator { get; set; }
    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    // Provider
    [JsonIgnore]
    public Guid ProviderId { get; set; }
    [JsonIgnore]
    public virtual Provider Provider { get; set; }

    public ICollection<AdministratorAction> AdministratorActions { get; set; } = new List<AdministratorAction>();
}
#nullable restore
