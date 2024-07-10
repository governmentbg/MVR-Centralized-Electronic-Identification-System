using MassTransit;

namespace eID.RO.Contracts.Events
{
    public interface LegalEntityBulstatCheckSucceeded : CorrelatedBy<Guid>
    {
        Guid EmpowermentId { get; }
    }
}
