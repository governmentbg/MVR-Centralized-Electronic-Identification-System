using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface VerifyLegalEntityWithdrawalRequester : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
    public string IssuerName { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
}
