using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface AdministratorUpdateUser : CorrelatedBy<Guid>
{
    public string AdministratorUid { get; set; }
    public IdentifierType AdministratorUidType { get; set; }
    public string AdministratorFullName { get; set; }

    public Guid ProviderId { get; set; }
    public Guid Id { get; set; }
    public bool IsAdministrator { get; set; }
    public string Comment { get; set; }
}
