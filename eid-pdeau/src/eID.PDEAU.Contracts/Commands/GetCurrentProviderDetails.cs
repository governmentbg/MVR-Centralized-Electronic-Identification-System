using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetCurrentProviderDetails : CorrelatedBy<Guid>
{
    Guid ProviderId { get; }
}
