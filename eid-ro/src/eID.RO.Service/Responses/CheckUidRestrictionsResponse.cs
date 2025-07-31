using eID.RO.Contracts.Results;

namespace eID.RO.Service.Responses;

public class CheckUidRestrictionsResponse : CheckUidRestrictionsResult
{
    public CheckUidRestrictionsState Response { get; set; }
    public virtual bool HasFailed { get; set; } = false;
    public virtual string? Error { get; set; }
}
