using MassTransit;

namespace eID.RO.Contracts.Events;

public interface VerifyLegalEntityWithdrawalRequesterFailed : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
}
