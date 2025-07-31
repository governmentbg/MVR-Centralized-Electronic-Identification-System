using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface TestNotificationChannel : CorrelatedBy<Guid>
{
    public Guid Id { get; }
}
