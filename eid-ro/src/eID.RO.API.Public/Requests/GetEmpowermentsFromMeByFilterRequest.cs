using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class GetEmpowermentsFromMeByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetEmpowermentsFromMeByFilterRequestValidator();
    /// <summary>
    /// Empowerment Status filter
    /// </summary>
    public EmpowermentsFromMeFilterStatus? Status { get; set; }
    /// <summary>
    /// Empowerment Authorizer name - contains
    /// </summary>
    public string? Authorizer { get; set; }
    /// <summary>
    /// Empowerment Supplier name
    /// </summary>
    public string? SupplierName { get; set; }
    /// <summary>
    /// Empowerment Service name or Service code - contains
    /// </summary>
    public string? ServiceName { get; set; }
    /// <summary>
    /// Empowerment Valid to Date filter
    /// </summary>
    public DateTime? ValidToDate { get; set; }
    /// <summary>
    /// Filter to show only never expiring empowerments
    /// </summary>
    public bool? ShowOnlyNoExpiryDate { get; set; }
    /// <summary>
    /// Filter to show only never expiring empowerments
    /// </summary>
    public List<UserIdentifierData>? EmpoweredUids { get; set; }
    /// <summary>
    /// Sort column
    /// </summary>
    public EmpowermentsFromMeSortBy? SortBy { get; set; }
    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection? SortDirection { get; set; }
    public OnBehalfOf? OnBehalfOf { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 1;
}
public class GetEmpowermentsFromMeByFilterRequestValidator : AbstractValidator<GetEmpowermentsFromMeByFilterRequest>
{
    public GetEmpowermentsFromMeByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.Status).IsInEnum();
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).NotNull().IsInEnum();
        });
        When(r => r.ValidToDate.HasValue, () =>
        {
            RuleFor(r => r.ShowOnlyNoExpiryDate).NotEqual(true);
        });
        When(r => r.ShowOnlyNoExpiryDate.HasValue && r.ShowOnlyNoExpiryDate.Value == true, () =>
        {
            RuleFor(r => r.ValidToDate).Equals(null);
        });
        When(r => r.OnBehalfOf.HasValue, () =>
        {
            RuleFor(r => r.OnBehalfOf).NotNull().IsInEnum();
        });
        When(r => r.EmpoweredUids != null, () =>
        {
            RuleFor(r => r.EmpoweredUids)
                .NotEmpty()
                .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
        });
    }
}
