using MassTransit;

namespace eID.RO.Contracts.Events;

public interface VerifyLegalEntityWithdrawalRequesterSucceeded : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
}
