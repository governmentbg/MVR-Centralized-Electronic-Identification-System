using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface RegisterProvider : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public string ExternalNumber { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public string CreatedByAdministratorId { get; set; }
    public string CreatedByAdministratorName { get; set; }
    public string Name { get; set; }
    public ProviderType Type { get; set; }
    public string Bulstat { get; set; }
    public string Headquarters { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public RegisterProviderUser Administrator { get; set; }
    public FilesInformation FilesInformation { get; set; }
}

public interface FilesInformation
{
    public string UploaderUid { get; set; }
    public IdentifierType UploaderUidType { get; set; }
    public string UploaderName { get; set; }
    public IEnumerable<FileData> Files { get; set; }
}

public class FileData
{
    public string FileName { get; set; }
    public string FullFilePath { get; set; }
}

public interface RegisterProviderUser 
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

