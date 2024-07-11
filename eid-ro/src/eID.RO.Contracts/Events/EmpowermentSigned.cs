using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Events;

public interface EmpowermentSigned : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
    string SignerName { get; }
    string SignerUid { get; }
    IdentifierType SignerUidType { get; }
}
