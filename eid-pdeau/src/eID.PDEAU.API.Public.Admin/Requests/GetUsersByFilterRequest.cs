using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class GetUsersByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetUsersByFilterValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    /// <summary>
    /// Searches the contents of Name for the given string
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Searches the contents of Email for the given string
    /// </summary>
    public string? Email { get; set; }
    public bool? IsAdministrator { get; set; }
    /// <summary>
    /// Sort column
    /// </summary>
    public UsersSortBy? SortBy { get; set; }
    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection? SortDirection { get; set; }
}

public class GetUsersByFilterValidator : AbstractValidator<GetUsersByFilterRequest>
{
    public GetUsersByFilterValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).NotNull().IsInEnum();
        });
    }
}
