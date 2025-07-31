using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderFileResult
{
    public Guid Id { get; set; }
    public string UploaderUid { get; set; }
    public IdentifierType UploaderUidType { get; set; }
    public string UploaderName { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedOn { get; set; }
}
