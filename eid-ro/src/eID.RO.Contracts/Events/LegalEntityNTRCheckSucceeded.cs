using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LegalEntityNTRCheckSucceeded : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
