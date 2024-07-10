using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface SignatureCollectionTimedOut : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
