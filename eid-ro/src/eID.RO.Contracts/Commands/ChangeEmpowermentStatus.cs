using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface ChangeEmpowermentStatus : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; set; }
    public EmpowermentStatementStatus Status { get; set; }
    public EmpowermentsDenialReason DenialReason { get; set; }
}
