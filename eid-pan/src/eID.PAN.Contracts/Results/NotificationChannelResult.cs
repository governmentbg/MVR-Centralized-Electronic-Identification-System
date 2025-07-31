namespace eID.PAN.Contracts.Results;

public interface NotificationChannelResult
{
    public Guid Id { get; set; }
    public string SystemName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string CallbackUrl { get; set; }
    public decimal Price { get; set; }
    public string Email { get; set; }
    public string InfoUrl { get; set; }
    public IEnumerable<NotificationChannelTranslationResult> Translations { get; set; }
}
