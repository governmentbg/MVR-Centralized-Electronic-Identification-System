namespace eID.Signing.Service.Objects;

public class EvrotrustWarningResponse
{
    public int Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}
