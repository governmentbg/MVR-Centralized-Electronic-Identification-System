using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Requests;
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
    /// <summary>
    /// Filter providers by number. Provider number template is ПДЕАУx/dd.mm.yyyy
    /// Where x is a integer and dd.mm.yyyy is the date of action
    /// Partial information can be applied
    /// </summary>
    public string? Number { get; set; }
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
