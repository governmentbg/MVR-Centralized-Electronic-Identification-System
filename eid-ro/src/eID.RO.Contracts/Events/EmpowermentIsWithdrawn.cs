using MassTransit;

namespace eID.RO.Contracts.Events;

public interface EmpowermentIsWithdrawn : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
