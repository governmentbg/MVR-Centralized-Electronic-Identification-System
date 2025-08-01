using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetDoneServicesByYear : CorrelatedBy<Guid>
{
    int Year { get; }
}
