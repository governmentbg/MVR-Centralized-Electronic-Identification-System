using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;
#nullable disable
public class GetProvidersByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetProvidersByFilterRequestValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
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
}

public class GetProvidersByFilterRequestValidator : AbstractValidator<GetProvidersByFilterRequest>
{
    public GetProvidersByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);

        When(r => r.Status.HasValue, () =>
        {
            RuleFor(r => r.Status).IsInEnum();
        });
    }
}
#nullable enable
