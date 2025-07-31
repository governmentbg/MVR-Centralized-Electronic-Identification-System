using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetUserAdministratorActions : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid UserId { get; set; }
}
