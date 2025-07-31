using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetAllServiceScopes : CorrelatedBy<Guid>
{
    Guid ServiceId { get; set; }
    Guid ProviderId { get; set; }
    bool IsPLSRole { get; set; }
}
