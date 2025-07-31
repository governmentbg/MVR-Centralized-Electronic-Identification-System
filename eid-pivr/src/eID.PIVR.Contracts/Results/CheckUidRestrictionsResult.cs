namespace eID.PIVR.Contracts.Results;

public interface CheckUidRestrictionsResult
{
    public CheckUidRestrictionsState Response { get; set; }
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}

public interface CheckUidRestrictionsState
{
    public bool IsProhibited { get; set; }
    public bool IsDead { get; set; }
    public bool HasRevokedParentalRights { get; set; }
}
