using eID.PIVR.Contracts.Enums;

namespace eID.PIVR.Contracts.Results;

public interface DeceasedByPeriodResult
{
    string PersonalId { get; }
    DateTime? Date { get; }
    UidType UidType { get; }
}
