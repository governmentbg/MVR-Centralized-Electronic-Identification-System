using eID.RO.Contracts.Commands;

namespace eID.RO.Contracts.Results;

public class LegalEntityVerificationResult
{
    /// <summary>
    /// Returns 'true' if Issuer is valid after external check for the legal entity
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// List of all representatives, that needs to sign the empowerment
    /// </summary>
    public IEnumerable<UserIdentifier> AuthorizerUids { get; set; } = new List<UserIdentifierData>();

    public LegalEntityVerificationResult() { }
    public LegalEntityVerificationResult(bool successful)
    {
        Successful = successful;
    }

    public LegalEntityVerificationResult(bool successful, IEnumerable<UserIdentifier> authorizerUids)
    {
        Successful = successful;
        AuthorizerUids = authorizerUids;
    }
}
