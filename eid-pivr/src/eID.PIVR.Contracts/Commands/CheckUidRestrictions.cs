using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface CheckUidRestrictions : CorrelatedBy<Guid>
{
    string Uid { get; }
    UidType UidType { get; }
}
