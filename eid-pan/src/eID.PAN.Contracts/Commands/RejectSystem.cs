using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RejectSystem : CorrelatedBy<Guid>
{
    public Guid SystemId { get; }
    public string UserId { get; set; }
    public string Reason { get; set; }
}
