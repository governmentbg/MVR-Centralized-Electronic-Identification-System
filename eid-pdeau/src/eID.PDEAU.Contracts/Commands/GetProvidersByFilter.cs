using eID.PDEAU.Contracts.Enums;
using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface GetProvidersByFilter : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public ProviderStatus? Status { get; set; }
    public string? ProviderName { get; set; }
    /// <summary>
    /// Sort column
    /// </summary>
    public ProvidersSortBy? SortBy { get; set; }
    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection? SortDirection { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string Number { get; set; }
    public bool IsPLSRole { get; set; }
}

public interface GetProvidersListByFilter : GetProvidersByFilter
{
}
