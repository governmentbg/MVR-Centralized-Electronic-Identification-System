using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface DenyEmpowermentByDeau : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    //Supplier
    public string AdministrationId { get; set; }
    public string DenialReasonComment { get; set; }
}
