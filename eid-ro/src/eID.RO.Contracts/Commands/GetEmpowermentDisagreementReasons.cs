using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentDisagreementReasons : CorrelatedBy<Guid>
{
}
