using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface SendChannelActivatedNotification : CorrelatedBy<Guid>
{
    Guid ChannelId { get; set; }
    Guid UserId { get; set; }
}
