using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LegalEntityBulstatCheckFailed : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
        public EmpowermentsDenialReason DenialReason { get; }
    }
}
