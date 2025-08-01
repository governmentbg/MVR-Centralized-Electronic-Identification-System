using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class GetAvailableProviderDetailsByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetAvailableProviderDetailsByFilterRequestValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    /// <summary>
    /// Searches the contents of Name for the given string
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Set to 'false' and the result will contain records regardless the services associated with them.
    /// </summary>
    public bool IncludeWithServicesOnly { get; set; } = true;
    /// <summary>
    /// Set to 'false' and the result will contain records even if there are no services that can be empowered.
    /// </summary>
    public bool IncludeEmpowermentOnly { get; set; } = true;
    /// <summary>
    /// Set to 'true' and the result will contain soft-deleted records.
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;
}

internal class GetAvailableProviderDetailsByFilterRequestValidator : AbstractValidator<GetAvailableProviderDetailsByFilterRequest>
{
    public GetAvailableProviderDetailsByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.Name).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.Name), ApplyConditionTo.CurrentValidator);
    }
}
