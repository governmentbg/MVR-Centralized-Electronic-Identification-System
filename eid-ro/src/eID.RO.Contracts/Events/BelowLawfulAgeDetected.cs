using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface BelowLawfulAgeDetected : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
