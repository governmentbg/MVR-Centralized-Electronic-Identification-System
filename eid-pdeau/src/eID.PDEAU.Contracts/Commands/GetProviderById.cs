using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderById : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public bool IsPLSRole { get; set; }
}
