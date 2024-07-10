using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface NoBelowLawfulAgeDetected : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
