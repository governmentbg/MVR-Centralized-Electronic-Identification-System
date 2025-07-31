using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface ApproveEmpowermentByDeau : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    public string ProviderId { get; set; }
}
