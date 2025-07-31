namespace eID.Signing.Contracts.Results;
public class EvrotrustStatusResult
{
    /// <summary>
    /// Possible values:
    /// 1 - Pending, 
    /// 2 - Signed, 
    /// 3 - Rejected, 
    /// 4 - Expired, 
    /// 5 - Failed, 
    /// 6 - Withdrawn, 
    /// 7 - Undeliverable, 
    /// 8 - Failed face recognition, 
    /// 99 - On hold;
    /// </summary>
    public int Status { get; set; }
    public bool IsProcessing { get; set; }
}
