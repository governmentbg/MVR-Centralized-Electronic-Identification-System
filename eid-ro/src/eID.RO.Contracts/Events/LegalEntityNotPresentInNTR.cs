using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LegalEntityNotPresentInNTR : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
