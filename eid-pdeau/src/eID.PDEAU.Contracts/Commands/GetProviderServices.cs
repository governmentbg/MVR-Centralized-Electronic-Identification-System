using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderServices : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
}
