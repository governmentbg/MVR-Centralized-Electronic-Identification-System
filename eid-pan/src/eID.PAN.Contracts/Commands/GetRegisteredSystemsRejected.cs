using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetRegisteredSystemsRejected : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
