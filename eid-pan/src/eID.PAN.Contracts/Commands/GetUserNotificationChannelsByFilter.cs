using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetUserNotificationChannelsByFilter : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string ChannelName { get; set; }
}
