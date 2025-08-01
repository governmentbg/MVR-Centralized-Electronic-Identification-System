using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetAllProviders : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
