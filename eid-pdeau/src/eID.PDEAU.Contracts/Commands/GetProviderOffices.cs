using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderOffices : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
