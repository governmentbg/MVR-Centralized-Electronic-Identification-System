using MassTransit;

namespace eID.RO.Contracts.Events;

public interface NoRestrictedUidsDetected : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
