using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface DeactivateService : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid UserId { get; set; }
    public string Uid { get; }
    public IdentifierType UidType { get; set; }
}
