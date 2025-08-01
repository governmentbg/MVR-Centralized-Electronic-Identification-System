using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderDetailsById : CorrelatedBy<Guid>
{
    Guid Id { get; set; }
}
