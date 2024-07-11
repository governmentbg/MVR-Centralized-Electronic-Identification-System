using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LawfulAgeInfoNotAvailable : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
