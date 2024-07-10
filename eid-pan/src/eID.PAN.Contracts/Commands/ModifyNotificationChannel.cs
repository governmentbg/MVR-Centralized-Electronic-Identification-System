using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface ModifyNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
    public Guid SystemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CallbackUrl { get; set; }
    public decimal Price { get; set; }
    public string InfoUrl { get; set; }
    public IEnumerable<NotificationChannelTranslation> Translations { get; set; }
}
