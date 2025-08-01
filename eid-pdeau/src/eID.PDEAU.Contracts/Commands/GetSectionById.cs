using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetSectionById : CorrelatedBy<Guid>
{
    Guid Id { get; set; }
}
