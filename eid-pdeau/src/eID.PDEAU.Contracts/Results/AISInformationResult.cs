using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface AISInformationResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }
}
