namespace eID.MIS.Contracts.SEV.Results;
public interface DeliveryRequestResult
{
    public Guid EIdentityId { get; set; }
    public DateTime SentOn { get; set; }
    public string SystemName { get; set; }
    public string Subject { get; set; }
    public string ReferencedOrn { get; set; }
    public int MessageId { get; set; }
}
