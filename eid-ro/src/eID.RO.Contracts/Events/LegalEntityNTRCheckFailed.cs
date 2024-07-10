using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LegalEntityNTRCheckFailed : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
        public EmpowermentsDenialReason DenialReason { get; }
    }
}
