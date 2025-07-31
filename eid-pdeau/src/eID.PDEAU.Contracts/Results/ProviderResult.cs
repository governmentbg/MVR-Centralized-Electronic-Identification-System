using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderResult
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public string Name { get; set; }
    public string Bulstat { get; set; }
    public string Headquarters { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string IdentificationNumber { get; set; }
    public ProviderStatus Status { get; set; }
    public ProviderType Type { get; set; }
    public DateTime CreatedOn { get; set; }
    public AISInformationResult AISInformation { get; set; }
    public IEnumerable<UserResult> Users { get; set; }
    public Guid? DetailsId { get; set; }
}

public interface ProviderWithFilesResult : ProviderResult
{
    public IEnumerable<ProviderFileResult> Files { get; set; }
}

public interface AdministratorRegisteredProviderResult : ProviderWithFilesResult
{
    public string ExternalNumber { get; set; }
    public string CreatedByAdministratorId { get; set; }
    public string CreatedByAdministratorName { get; set; }
}

public interface ProviderListResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ProviderType Type { get; set; }
}
