using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RegisterUserNotificationChannels : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    HashSet<Guid> Ids { get; set; }
    public string ModifiedBy { get; set; }
}
