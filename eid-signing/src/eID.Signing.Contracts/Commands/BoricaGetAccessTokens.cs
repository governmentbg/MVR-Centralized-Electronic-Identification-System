using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaGetAccessTokens : CorrelatedBy<Guid>
{
}

