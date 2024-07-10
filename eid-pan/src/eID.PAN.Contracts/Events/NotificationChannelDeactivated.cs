using MassTransit;

namespace eID.PAN.Contracts.Events
{
    /// <summary>
    /// When a notification channel is deactivated
    /// the system notifies all users that have it selected. The fallback channel is email.
    /// </summary>
    public interface NotificationChannelDeactivated : CorrelatedBy<Guid>
    {
        public Guid NotificationChannelId { get; }
    }
}
