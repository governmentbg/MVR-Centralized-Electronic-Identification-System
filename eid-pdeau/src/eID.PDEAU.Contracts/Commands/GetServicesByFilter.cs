using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetServicesByFilter : CorrelatedBy<Guid>
{
    int PageSize { get; set; }
    int PageIndex { get; set; }
    int? ServiceNumber { get; set; }
    string Name { get; set; }
    string Description { get; set; }
    string FindServiceNumberAndName { get; set; }
    bool IncludeEmpowermentOnly { get; set; }
    bool IncludeDeleted { get; set; }
    bool IncludeWithoutScope { get; set; }
    bool IncludeInactive { get; set; }
    bool IncludeApprovedOnly { get; set; }
    Guid? ProviderId { get; set; }
    Guid? ProviderSectionId { get; set; }
    public ProviderServicesSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
    public bool IsPLSRole { get; set; }
}
