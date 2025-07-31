using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProviderDetailsByFilter : CorrelatedBy<Guid>
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    string Name { get; set; }
    bool IncludeWithServicesOnly { get; set; }
    bool IncludeEmpowermentOnly { get; set; }
    bool IncludeDeleted { get; set; }
    ProviderDetailsStatus Status { get; set; }
}
public interface GetAvailableProviderDetailsByFilter : CorrelatedBy<Guid>
{
}
