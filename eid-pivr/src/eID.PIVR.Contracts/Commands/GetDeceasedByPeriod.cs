using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface GetDeceasedByPeriod : CorrelatedBy<Guid>
{
    DateTime From { get; }
    DateTime To { get; }
}
