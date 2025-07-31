using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderStatusHistory : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public bool IsPLSRole { get; set; }
}
