using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetUserNotificationChannels : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public Guid UserId { get; set; }
}
