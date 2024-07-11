using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface ChangeEmpowermentWithdrawalStatus : CorrelatedBy<Guid>
{
    Guid EmpowermentWithdrawalId { get; }
    EmpowermentWithdrawalStatus Status { get; }
}
