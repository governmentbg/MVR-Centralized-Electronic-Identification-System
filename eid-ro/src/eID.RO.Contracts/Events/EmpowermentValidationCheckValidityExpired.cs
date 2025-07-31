using MassTransit;

namespace eID.RO.Contracts.Events;

public interface EmpowermentValidationCheckValidityExpired : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
