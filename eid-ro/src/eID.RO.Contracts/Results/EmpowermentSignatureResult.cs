namespace eID.RO.Contracts.Results;

public interface EmpowermentSignatureResult
{
    /// <summary>
    /// Signing datetime (UTC)
    /// </summary>
    public DateTime DateTime { get; }
    /// <summary>
    /// Person who's signature is being stored
    /// </summary>
    public string SignerUid { get; }
    /// <summary>
    /// The detached signature
    /// </summary>
    public string Signature { get; }
}
