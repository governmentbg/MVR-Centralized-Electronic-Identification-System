namespace eID.Signing.Service.Objects;

public class EvrotrustDocSigningResponse
{
    public SignResponse? Response { get; set; }
    public bool GroupSigning { get; set; }
}

public class SignResponse
{
    public string ThreadID { get; set; } = string.Empty;
    public IEnumerable<SignResponseTransaction>? Transactions { get; set; }
}

public class SignResponseTransaction
{
    public string TransactionID { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
}
