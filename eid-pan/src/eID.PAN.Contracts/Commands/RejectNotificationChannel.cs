using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RejectNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
}
