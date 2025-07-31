using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface AdministratorRegisterUser : CorrelatedBy<Guid>
{
    public string AdministratorUid { get; set; }
    public IdentifierType AdministratorUidType { get; set; }
    public string AdministratorFullName { get; set; }
    public string Comment { get; set; }

    public Guid ProviderId { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsAdministrator { get; set; }
}
