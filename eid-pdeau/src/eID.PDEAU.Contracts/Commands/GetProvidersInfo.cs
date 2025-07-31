using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProvidersInfo : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string Name { get; set; }
    public string IdentificationNumber { get; set; }
    public string Bulstat { get; set; }
    public GetProvidersInfoSortBy SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public enum GetProvidersInfoSortBy
{
    None = 0,
    Name = 1,
    IdentificationNumber = 2,
    Bulstat = 3,
}
