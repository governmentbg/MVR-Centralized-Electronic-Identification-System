#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Entities;

public class AISInformation : AISInformationResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }

    //Provider
    [JsonIgnore]
    public Guid ProviderId { get; set; }
    [JsonIgnore]  
    public virtual Provider Provider { get; set; }
}
#nullable restore
