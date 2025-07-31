namespace eID.Signing.Contracts.Results;

public class EvrotrustDocumentSigningResult
{
    public string ThreadID { get; set; }
    public bool GroupSigning { get; set; }
    public IEnumerable<EvrotrustDocumentTransactionResult> Transactions { get; set; }
}
