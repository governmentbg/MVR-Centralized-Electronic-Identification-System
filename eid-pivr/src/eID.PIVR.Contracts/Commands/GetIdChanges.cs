using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands;

/// <summary>
/// Get UId changes.
/// If there are PersonalId and UidType -> CreatedOnFrom and CreatedOnTo can be mandatory.
/// If there are CreatedOnFrom and CreatedOnTo -> PersonalId and UidType can be mandatory.
/// Also all fields can be filled
/// </summary>
public interface GetIdChanges : CorrelatedBy<Guid>
{
    string? PersonalId { get; }
    UidType? UidType { get; }

    DateTime? CreatedOnFrom { get; }
    DateTime? CreatedOnTo { get; }
}
