using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface ApproveSystem : CorrelatedBy<Guid>
{
    public Guid SystemId { get; }
    public string UserId { get; set; }
}
