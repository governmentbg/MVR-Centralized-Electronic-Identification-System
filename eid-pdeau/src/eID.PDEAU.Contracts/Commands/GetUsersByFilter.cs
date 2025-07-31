using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;
#nullable disable
public interface GetUsersByFilter : CorrelatedBy<Guid>
{
    public Guid ProviderId { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? IsAdministrator { get; set; }
    public UsersSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}
#nullable restore
