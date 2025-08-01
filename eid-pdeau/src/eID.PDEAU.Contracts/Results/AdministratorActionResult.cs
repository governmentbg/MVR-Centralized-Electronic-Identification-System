using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface AdministratorActionResult
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string AdministratorUid { get; set; }
    public IdentifierType AdministratorUidType { get; set; }
    public string AdministratorFullName { get; set; }
    public AdministratorActionType Action { get; set; }
    public string Comment { get; set; }
}
