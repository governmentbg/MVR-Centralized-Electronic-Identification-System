using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface TimestampEmpowermentWithdrawal : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    public Guid EmpowermentWithdrawalId { get; set; }
}
