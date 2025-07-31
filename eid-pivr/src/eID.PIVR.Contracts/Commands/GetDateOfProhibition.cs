using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface GetDateOfProhibition : CorrelatedBy<Guid>
{
    string PersonalId { get; }
    /// <summary>
    /// Uid type: EGN or LNCh
    /// </summary>
    UidType UidType { get; }
}
