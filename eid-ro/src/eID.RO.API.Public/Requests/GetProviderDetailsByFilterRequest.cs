using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class GetProviderDetailsByFilterRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    /// <summary>
    /// Searches the contents of Name for the given string
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Set to 'true' and the result will contain soft-deleted records.
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;
    public ProviderDetailsStatus Status { get; set; } = ProviderDetailsStatus.Active;
}

