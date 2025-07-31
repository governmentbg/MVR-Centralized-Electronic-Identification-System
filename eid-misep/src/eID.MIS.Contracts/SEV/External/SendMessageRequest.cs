namespace eID.MIS.Contracts.SEV.External;

public class SendMessageRequest
{
    public List<string> RecipientProfileIds { get; set; }
    public string Subject { get; set; }
    public string ReferencedOrn { get; set; }
    public string AdditionalIdentifier { get; set; }
    public string TemplateId { get; set; }
    public Dictionary<string, object> Fields { get; set; }
}


