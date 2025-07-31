using MassTransit;

namespace eID.RO.Contracts.Events;

public interface RegistrationStatusAllAvailable : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}

