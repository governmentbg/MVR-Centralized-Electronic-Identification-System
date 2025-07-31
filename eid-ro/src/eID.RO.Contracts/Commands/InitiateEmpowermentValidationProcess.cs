using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface InitiateEmpowermentValidationProcess : CorrelatedBy<Guid>
{
    public Guid EmpowermentId { get; }
    public OnBehalfOf OnBehalfOf { get; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string IssuerPosition { get; set; }
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; }
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }
}
