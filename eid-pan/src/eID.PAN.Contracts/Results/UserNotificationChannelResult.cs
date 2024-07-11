namespace eID.PAN.Contracts.Results;

public interface UserNotificationChannelResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string InfoUrl { get; set; }
    public IEnumerable<NotificationChannelTranslationResult> Translations { get; set; }
}
