using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface ApproveEmpowermentByDeau : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    //Supplier
    public string AdministrationId { get; set; }
}
