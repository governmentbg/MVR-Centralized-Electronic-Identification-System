using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface GetApiUsageByYear : CorrelatedBy<Guid>
{
    int Year { get; }
}
