using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class GetProviderInfoRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetProviderInfoRequestValidator();
    
    /// <summary>
    /// Page index. Default 1
    /// </summary>
    public int PageIndex { get; set; } = 1;
    
    /// <summary>
    /// Page size. Default 100. Maximum 100
    /// </summary>
    public int PageSize { get; set; } = 100;

    public string? Name { get; set; }
    public string? IdentificationNumber { get; set; }
    public string? Bulstat { get; set; }
    
    /// <summary>
    /// Sorted by. Default None. Not required
    /// </summary>
    public GetProvidersInfoSortBy SortBy { get; set; }
    
    /// <summary>
    /// Sort direction. Default ascending. Not required
    /// </summary>
    public SortDirection SortDirection { get; set; }
}

internal class GetProviderInfoRequestValidator : AbstractValidator<GetProviderInfoRequest>
{
    public GetProviderInfoRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
        RuleFor(r => r.SortBy).IsInEnum();
        RuleFor(r => r.SortDirection).IsInEnum();
    }
}
