using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface TimestampEmpowermentXml : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
}
