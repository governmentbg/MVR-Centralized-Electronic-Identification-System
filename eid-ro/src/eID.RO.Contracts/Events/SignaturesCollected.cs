using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface SignaturesCollected : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
