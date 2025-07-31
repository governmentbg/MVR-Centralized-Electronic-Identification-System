using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Requests;

public class GetEmpowermentsByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetEmpowermentsByFilterRequestValidator();
    /// <summary>
    /// Filter on empowerment number. Empowerment number template is РОx/dd.mm.yyyy
    /// Where x is a integer and dd.mm.yyyy is the date of action
    /// Partial information can be applied
    /// </summary>
    public string? Number { get; set; }
    /// <summary>
    /// Filter on empowerment status
    /// </summary>
    public EmpowermentsFromMeFilterStatus? Status { get; set; }
    /// <summary>
    /// Filter on Authorizer name
    /// The entered string is searched for a partial match
    /// </summary>
    public string? Authorizer { get; set; }
    /// <summary>
    /// Filters empowerments created on or after the provided date.
    /// </summary>
    public DateTime? CreatedOnFrom { get; set; }
    /// <summary>
    /// Filters empowerments created on or before the provided date.
    /// </summary>
    public DateTime? CreatedOnTo { get; set; }
    /// <summary>
    /// Filter on provider name. The operation is case insensitive
    /// </summary>
    public string? ProviderName { get; set; }
    /// <summary>
    /// Filter on service name or code. The filter is performed using a partial match.
    /// </summary>
    public string? ServiceName { get; set; }
    /// <summary>
    /// Retrieves empowerments with a non-null ExpiryDate that is less than or equal to the provided ValidToDate (converted to UTC)
    /// </summary>
    public DateTime? ValidToDate { get; set; }
    /// <summary>
    /// Set to true to get only non-expiring empowerments.
    /// </summary>
    public bool? ShowOnlyNoExpiryDate { get; set; }
    /// <summary>
    /// Filter on empowered Uids
    /// </summary>
    public List<UidAndUidTypeData>? EmpoweredUids { get; set; }
    /// <summary>
    /// Result will be sorted by the selected property
    /// </summary>
    public EmpowermentsFromMeSortBy? SortBy { get; set; }
    /// <summary>
    /// Result will be sorted through direction provided here. This operation is linked with SortBy filter
    /// </summary>
    public SortDirection? SortDirection { get; set; }
    /// <summary>
    /// The legal type of the authorizer
    /// </summary>
    public OnBehalfOf? OnBehalfOf { get; set; }
    /// <summary>
    /// Filter on empowerment uid. The filter is performed using a exact match.
    /// </summary>
    public string? EmpowermentUid { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 1;
}
public class GetEmpowermentsByFilterRequestValidator : AbstractValidator<GetEmpowermentsByFilterRequest>
{
    public GetEmpowermentsByFilterRequestValidator()
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
                .ForEach(r => r.SetValidator(new UidAndTypeValidator()));
        });

        When(r => r.OnBehalfOf.HasValue, () =>
        {
            RuleFor(r => r.OnBehalfOf).IsInEnum();
        });

        When(r => !string.IsNullOrWhiteSpace(r.EmpowermentUid), () =>
        {
            RuleFor(x => x.EmpowermentUid)
                .Cascade(CascadeMode.Stop)
                .Must(x => ValidatorHelpers.EikFormatIsValid(x) || ValidatorHelpers.UidFormatIsValid(x))
                    .WithMessage("{PropertyName} contains invalid Uid.");
        });

        When(x => x.CreatedOnFrom.HasValue, () =>
        {
            RuleFor(r => r.CreatedOnFrom).LessThanOrEqualTo(DateTime.UtcNow);
        });

        When(x => x.CreatedOnTo.HasValue, () =>
        {
            RuleFor(r => r.CreatedOnTo).LessThan(DateTime.UtcNow.Date.AddDays(1));
        });
    }
}
