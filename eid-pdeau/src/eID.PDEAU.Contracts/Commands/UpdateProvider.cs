using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface UpdateProvider : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    /// <summary>
    /// Update comment, what was done.
    /// </summary>
    public string Comment { get; set; }
    public FilesInformation FilesInformation { get; set; }
}

public interface UpdateProviderUser
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public interface UpdateProviderAISInformation
{
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }
}
