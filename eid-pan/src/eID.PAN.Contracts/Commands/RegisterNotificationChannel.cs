using eID.PAN.Contracts.Results;
using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RegisterNotificationChannel : CorrelatedBy<Guid>
{
    public string ModifiedBy { get; set; }
    public Guid SystemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CallbackUrl { get; set; }
    public decimal Price { get; set; }
    public string InfoUrl { get; set; }
    public IEnumerable<NotificationChannelTranslation> Translations { get; set; }
}

public interface NotificationChannelTranslation
{
    public string Language { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
