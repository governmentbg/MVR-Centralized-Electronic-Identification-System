using MassTransit;

namespace eID.RO.Contracts.Events;

public interface TimestampEmpowermentXmlSucceeded : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
