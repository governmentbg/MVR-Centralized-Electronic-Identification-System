using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderFile : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public Guid FileId { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public bool IsPublic { get; set; }
}
