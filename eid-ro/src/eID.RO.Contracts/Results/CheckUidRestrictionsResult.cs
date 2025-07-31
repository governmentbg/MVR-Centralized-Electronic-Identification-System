namespace eID.RO.Contracts.Results;

public interface CheckUidRestrictionsResult
{
    public CheckUidRestrictionsState Response { get; set; }
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}

public class CheckUidRestrictionsState
{
    public bool IsProhibited { get; set; }
    public bool IsDead { get; set; }
}
