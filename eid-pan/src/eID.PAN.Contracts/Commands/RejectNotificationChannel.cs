using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RejectNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string ModifiedBy { get; set; }
    public string Reason { get; set; }
}
