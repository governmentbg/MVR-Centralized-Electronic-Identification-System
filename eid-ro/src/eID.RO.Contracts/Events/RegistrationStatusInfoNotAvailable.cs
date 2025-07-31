using MassTransit;

namespace eID.RO.Contracts.Events;

public interface RegistrationStatusInfoNotAvailable : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}

