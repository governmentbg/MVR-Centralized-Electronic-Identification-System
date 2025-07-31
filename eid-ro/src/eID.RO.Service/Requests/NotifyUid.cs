using eID.RO.Contracts.Enums;

namespace eID.RO.Service.Requests;

public class NotifyUid
{
    public Guid CorrelationId { get; set; }
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string EventCode { get; set; } = string.Empty;
}
