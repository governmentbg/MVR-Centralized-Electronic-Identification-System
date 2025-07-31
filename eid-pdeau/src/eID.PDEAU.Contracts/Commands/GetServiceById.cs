using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetServiceById : CorrelatedBy<Guid>
{
    Guid Id { get; set; }
}
