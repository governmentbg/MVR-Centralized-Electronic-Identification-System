using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendPushNotification : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public IEnumerable<PushNotificationTranslation> Translations { get; set; }
}

public interface PushNotificationTranslation
{
    public string Language { get; set; }
    public string Message { get; set; }
}
