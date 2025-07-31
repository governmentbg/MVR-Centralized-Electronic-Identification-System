using eID.PIVR.Contracts.Enums;

namespace eID.PIVR.Contracts.Results;

public interface StatutChangeResult
{
    string PersonalId { get; }

    UidType UidType { get; }

    /// <summary>
    /// Date of change
    /// </summary>
    DateTime Date { get; }

    DateTime CreatedOn { get; }
}
