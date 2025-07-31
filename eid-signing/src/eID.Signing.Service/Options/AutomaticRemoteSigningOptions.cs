namespace eID.Signing.Service.Options;

public class AutomaticRemoteSigningOptions
{
    public string AuthorizedPersonCertId { get; set; }
    public int ExpirationTimeInDays { get; set; }
    public Organisation Organisation { get; set; }
    public string Role { get; set; }
    public string Subject { get; set; }
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AuthorizedPersonCertId))
        {
            throw new ArgumentNullException(nameof(AuthorizedPersonCertId), $"{nameof(AuthorizedPersonCertId)} is required.");
        }

        if (ExpirationTimeInDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ExpirationTimeInDays), $"{nameof(ExpirationTimeInDays)} must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(Role))
        {
            throw new ArgumentNullException(nameof(Role), $"{nameof(Role)} is required.");
        }

        if (string.IsNullOrWhiteSpace(Subject))
        {
            throw new ArgumentNullException(nameof(Subject), $"{nameof(Subject)} is required.");
        }

        if (Organisation is null)
        {
            throw new ArgumentNullException(nameof(Organisation), $"{nameof(Organisation)} is required.");
        }

        if (string.IsNullOrWhiteSpace(Organisation.OrgIdentifier))
        {
            throw new ArgumentNullException(nameof(Organisation.OrgIdentifier), $"{nameof(Organisation.OrgIdentifier)} is required.");
        }

        if (string.IsNullOrWhiteSpace(Organisation.OrgIdentifierType))
        {
            throw new ArgumentNullException(nameof(Organisation.OrgIdentifierType), $"{nameof(Organisation.OrgIdentifierType)} is required.");
        }

        if (string.IsNullOrWhiteSpace(Organisation.OrgName))
        {
            throw new ArgumentNullException(nameof(Organisation.OrgName), $"{nameof(Organisation.OrgName)} is required.");
        }
    }
}

public class Organisation
{
    public string OrgIdentifier { get; set; }
    public string OrgIdentifierType { get; set; } = "BULSTAT";
    public string OrgName { get; set; }
}
