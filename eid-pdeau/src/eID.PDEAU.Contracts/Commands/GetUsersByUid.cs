using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetUserByUid : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
}
