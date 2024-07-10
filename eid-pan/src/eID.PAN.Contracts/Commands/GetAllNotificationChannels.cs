using MassTransit;

namespace eID.PAN.Contracts.Commands
{
    public interface GetAllNotificationChannels : CorrelatedBy<Guid>
    {
        public string UserId { get; set; }
    }
}
