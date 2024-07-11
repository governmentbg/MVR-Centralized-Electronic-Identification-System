using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface RegisterDeactivatedEvents : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    HashSet<Guid> Ids { get; set; }
    public string ModifiedBy { get; set; }
}
