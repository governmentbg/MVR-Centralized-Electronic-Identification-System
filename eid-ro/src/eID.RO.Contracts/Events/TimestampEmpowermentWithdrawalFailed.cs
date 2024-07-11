using MassTransit;

namespace eID.RO.Contracts.Events;

public interface TimestampEmpowermentWithdrawalFailed : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
    Guid EmpowermentWithdrawalId { get; }
}
