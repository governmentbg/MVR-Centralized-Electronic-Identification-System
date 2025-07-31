using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetSectionsByFilter : CorrelatedBy<Guid>
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    string Name { get; set; }
    bool IncludeDeleted { get; set; }
}
