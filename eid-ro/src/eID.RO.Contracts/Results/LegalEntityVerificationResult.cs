using eID.RO.Contracts.Commands;

namespace eID.RO.Contracts.Results;

public class LegalEntityVerificationResult
{
    /// <summary>
    /// Returns 'true' if Issuer is valid after external check for the legal entity
    /// </summary>
    public bool Successfull { get; set; }

    /// <summary>
    /// List of all representatives, that needs to sign the empowerment
    /// </summary>
    public IEnumerable<UserIdentifier> AuthorizerUids { get; set; } = new List<UserIdentifierData>();

    public LegalEntityVerificationResult() { }
    public LegalEntityVerificationResult(bool successfull)
    {
        Successfull = successfull;
    }

    public LegalEntityVerificationResult(bool successfull, IEnumerable<UserIdentifier> authorizerUids)
    {
        Successfull = successfull;
        AuthorizerUids = authorizerUids;
    }
}
