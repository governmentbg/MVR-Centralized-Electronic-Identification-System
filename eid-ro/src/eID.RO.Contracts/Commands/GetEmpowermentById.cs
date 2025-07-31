using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentById : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
