namespace eID.PAN.Service.Requests;

public class SendNotificationRequest
{
    public Guid UserId { get; set; }
    public IEnumerable<SendNotificationContentTranslation> Translations { get; set; } = new List<SendNotificationContentTranslation>();
}

public class SendNotificationContentTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}
