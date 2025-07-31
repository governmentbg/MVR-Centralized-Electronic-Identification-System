using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaCheckConsentStatus : CorrelatedBy<Guid>
{
    public string CallbackId { get; set; }
}

