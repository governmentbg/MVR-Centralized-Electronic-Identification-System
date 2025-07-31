using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface AddEmpowermentStatement : CorrelatedBy<Guid>
{
    OnBehalfOf OnBehalfOf { get; }
    string Name { get; }
    string Uid { get; }
    IdentifierType UidType { get; }
    IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; }
    IEnumerable<UserIdentifier> EmpoweredUids { get; }
    TypeOfEmpowerment TypeOfEmpowerment { get; }
    string ProviderId { get; }
    string ProviderName { get; }
    int ServiceId { get; }
    string ServiceName { get; }
    string IssuerPosition { get; }
    IEnumerable<VolumeOfRepresentationResult> VolumeOfRepresentation { get; }
    DateTime StartDate { get; }
    DateTime? ExpiryDate { get; }
    string CreatedBy { get; }
    bool AllowSelfEmpowerment { get; }
}
