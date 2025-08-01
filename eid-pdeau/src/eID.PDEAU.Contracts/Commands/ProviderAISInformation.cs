using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Commands;

public interface ProviderAISInformation
{
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }
}

