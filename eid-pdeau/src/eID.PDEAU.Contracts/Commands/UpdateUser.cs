using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface UpdateUser : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid Id { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsAdministrator { get; set; }
}
