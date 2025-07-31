using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface CheckForEmpowermentsVerification : CorrelatedBy<Guid>
{
    public IEnumerable<Guid> EmpowermentIds { get; }
}
