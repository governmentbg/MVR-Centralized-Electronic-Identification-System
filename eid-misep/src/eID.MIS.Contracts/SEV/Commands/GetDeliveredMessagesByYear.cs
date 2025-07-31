using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface GetDeliveredMessagesByYear : CorrelatedBy<Guid>
{
    int Year { get; }
}
