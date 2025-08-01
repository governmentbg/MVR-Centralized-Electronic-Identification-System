#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Entities;

public class ProviderFile : ProviderFileResult
{
    public Guid Id { get; set; }
    public string UploaderUid { get; set; }
    public IdentifierType UploaderUidType { get; set; }
    public string UploaderName { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedOn { get; set; }


    //Provider
    [JsonIgnore]
    public Guid ProviderId { get; set; }
    [JsonIgnore]
    public virtual Provider Provider { get; set; }
}
#nullable restore
