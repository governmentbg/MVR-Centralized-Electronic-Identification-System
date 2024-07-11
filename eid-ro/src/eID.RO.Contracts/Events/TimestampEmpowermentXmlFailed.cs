using MassTransit;

namespace eID.RO.Contracts.Events;
public interface TimestampEmpowermentXmlFailed : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
