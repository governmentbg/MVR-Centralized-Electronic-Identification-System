using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface DenyProvider : CorrelatedBy<Guid>
{
    Guid ProviderId { get; set; }
    string IssuerUid { get; set; }
    IdentifierType IssuerUidType { get; set; }
    string IssuerName { get; set; }
    string Comment { get; set; }
    bool IsPLSRole { get; set; }
}
