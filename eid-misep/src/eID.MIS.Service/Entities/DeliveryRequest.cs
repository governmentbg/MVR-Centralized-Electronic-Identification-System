using eID.MIS.Contracts.SEV.Results;

namespace eID.MIS.Service.Entities;

public class DeliveryRequest : DeliveryRequestResult

{
    public Guid Id { get; set; }
    public Guid EIdentityId { get; set; }
    public DateTime SentOn { get; set; }
    public string SystemName { get; set; }
    public string Subject { get; set; }
    public string ReferencedOrn { get; set; }
    public int MessageId { get; set; }
}
