namespace eID.Signing.Contracts.Results;
public class EvrotrustUserCheckResult
{
    public bool IsRegistered { get; set; }
    public bool IsIdentified { get; set; }
    public bool IsRejected { get; set; }
    public bool IsSupervised { get; set; }
    public bool IsReadyToSign { get; set; }
    public bool HasConfirmedPhone { get; set; }
    public bool HasConfirmedEmail { get; set; }
}
