using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RestoreNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
}
