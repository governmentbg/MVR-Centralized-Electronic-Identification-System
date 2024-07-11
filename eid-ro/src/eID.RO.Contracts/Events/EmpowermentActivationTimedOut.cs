using MassTransit;

namespace eID.RO.Contracts.Events;

public interface EmpowermentActivationTimedOut : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
