using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetActiveProviders : CorrelatedBy<Guid>
{
}
