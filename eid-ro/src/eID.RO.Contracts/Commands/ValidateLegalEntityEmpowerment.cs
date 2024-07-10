using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface ValidateLegalEntityEmpowerment : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
    public IEnumerable<UserIdentifierWithName> AuthorizerUids { get; set; }
}
