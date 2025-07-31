using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface InitiateAdminPromotion : CorrelatedBy<Guid>
{
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public Guid ProviderId { get; set; }
    public Guid UserId { get; set; }
}
