using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetAvailableScopesByProviderId : CorrelatedBy<Guid>
{
    Guid ProviderId { get; set; }
}
