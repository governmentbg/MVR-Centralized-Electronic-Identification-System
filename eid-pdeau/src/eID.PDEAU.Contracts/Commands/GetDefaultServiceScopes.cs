using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetDefaultServiceScopes : CorrelatedBy<Guid>
{
}
