using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface DeleteUser : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid Id { get; set; }
}
