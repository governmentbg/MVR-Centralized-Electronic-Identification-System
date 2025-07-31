using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface ActivateService : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid ServiceId { get; }
    public Guid UserId { get; set; }
    public string Uid { get; }
    public IdentifierType UidType { get; set; }
}
