namespace eID.MIS.Service;

public class BaseResponse
{
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}
