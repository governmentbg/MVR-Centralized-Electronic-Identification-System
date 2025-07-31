using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface UserResult
{
    public Guid Id { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsAdministrator { get; set; }
    public DateTime CreatedOn { get; set; }
}
