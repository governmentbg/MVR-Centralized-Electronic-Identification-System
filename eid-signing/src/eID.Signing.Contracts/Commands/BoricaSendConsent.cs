using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaSendConsent : CorrelatedBy<Guid>
{
}

