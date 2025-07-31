using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface GetDateOfDeath : CorrelatedBy<Guid>
{
    string PersonalId { get; }
    UidType UidType { get; }
}
