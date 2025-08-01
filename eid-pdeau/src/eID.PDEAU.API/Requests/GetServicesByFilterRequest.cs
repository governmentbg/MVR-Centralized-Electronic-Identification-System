using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class GetServicesByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetServicesByFilterRequestValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public int? ServiceNumber { get; set; }
    /// <summary>
    /// Searches the contents of Name for the given string
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Searches the contents of Description for the given string
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Searches the service number and name for the given string
    /// </summary>
    public string? FindServiceNumberAndName { get; set; }
    /// <summary>
    /// Set to 'true' to return only services that can be empowered.
    /// </summary>
    public bool IncludeEmpowermentOnly { get; set; } = true;
    /// <summary>
    /// Set to 'true' and the result will contain soft-deleted records.
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;
    /// <summary>
    /// Set to 'true' and the result will contain records without any scopes associated to them.
    /// </summary>
    public bool IncludeWithoutScope { get; set; } = false;
    /// <summary>
    /// Set to 'true' and the result will contain inactive records.
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
    /// <summary>
    /// Set to 'false' and the result will contain inactive records.
    /// </summary>
    public bool IncludeApprovedOnly { get; set; } = true;
    public Guid? ProviderId { get; set; }
    public Guid? ProviderSectionId { get; set; }
    public ProviderServicesSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

internal class GetServicesByFilterRequestValidator : AbstractValidator<GetServicesByFilterRequest>
{
    public GetServicesByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.Name).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.Name), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.Description).MaximumLength(512)
            .When(r => !string.IsNullOrEmpty(r.Description), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.FindServiceNumberAndName).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.FindServiceNumberAndName), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).NotNull().IsInEnum();
        });
    }
}
