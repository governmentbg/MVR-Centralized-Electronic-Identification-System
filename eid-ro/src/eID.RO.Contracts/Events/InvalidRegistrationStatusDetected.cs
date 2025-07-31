using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Events;

public interface InvalidRegistrationStatusDetected : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
    public EmpowermentsDenialReason DenialReason { get; }
}

