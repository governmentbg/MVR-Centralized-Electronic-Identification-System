using MassTransit;

namespace eID.RO.Contracts.Events;

public interface TimestampEmpowermentWithdrawalSucceeded : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
    Guid EmpowermentWithdrawalId { get; }
}
