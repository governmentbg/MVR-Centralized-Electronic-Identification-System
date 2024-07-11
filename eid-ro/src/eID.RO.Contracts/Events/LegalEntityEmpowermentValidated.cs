using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Events;

public interface LegalEntityEmpowermentValidated : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
}
public interface LegalEntityEmpowermentValidationFailed : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
    public EmpowermentsDenialReason DenialReason { get; }
}
