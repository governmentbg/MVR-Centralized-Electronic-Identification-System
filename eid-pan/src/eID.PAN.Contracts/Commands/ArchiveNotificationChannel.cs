using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface ArchiveNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
    public string ModifiedBy { get; set; }
}
