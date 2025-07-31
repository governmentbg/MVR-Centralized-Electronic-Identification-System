using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface RegisterDoneService : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public string ServiceName { get; set; }
    public int Count { get; set; }
}
