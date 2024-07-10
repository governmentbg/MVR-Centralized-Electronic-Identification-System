using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetNotificationsByFilter : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string SystemName { get; set; }
    public bool IncludeDeleted { get; set; }
}
