using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentWithdrawReasons : CorrelatedBy<Guid>
{
}
