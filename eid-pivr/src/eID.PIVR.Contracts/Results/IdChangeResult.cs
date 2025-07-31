using eID.PIVR.Contracts.Enums;

namespace eID.PIVR.Contracts.Results;

public interface IdChangeResult
{
    string OldPersonalId { get; }

    UidType OldUidType { get; }

    string NewPersonalId { get; }

    UidType NewUidType { get; }

    /// <summary>
    /// Date of change
    /// </summary>
    DateTime Date { get; }

    DateTime CreatedOn { get; }
}
