using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetActivatedEmpowermentsByYear : CorrelatedBy<Guid>
{
    int Year { get; }
}
