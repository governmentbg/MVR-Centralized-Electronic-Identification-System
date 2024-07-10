using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface ApproveNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
}
