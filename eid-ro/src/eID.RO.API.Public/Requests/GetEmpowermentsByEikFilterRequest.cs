using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class GetEmpowermentsByEikFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetEmpowermentsByEikFilterRequestValidator();

    public string Eik { get; set; } = string.Empty;
    public EmpowermentsByEikFilterStatus? Status { get; set; }
    public string? ProviderName { get; set; }
    public string? ServiceName { get; set; }
    public DateTime? ValidToDate { get; set; }
    public bool? ShowOnlyNoExpiryDate { get; set; }
    public List<UserIdentifierData>? AuthorizerUids { get; set; }
    public EmpowermentsByEikSortBy? SortBy { get; set; }
    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection? SortDirection { get; set; }

    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 1;
}

public class GetEmpowermentsByEikFilterRequestValidator : AbstractValidator<GetEmpowermentsByEikFilterRequest>
{
    public GetEmpowermentsByEikFilterRequestValidator()
    {
        RuleFor(r => r.Eik)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid Eik."); 
        
        When(r => r.AuthorizerUids != null, () =>
        {
            RuleFor(r => r.AuthorizerUids)
                .NotEmpty()
                .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
        });

        When(r => r.ShowOnlyNoExpiryDate.HasValue && r.ShowOnlyNoExpiryDate == true, () =>
        {
            RuleFor(r => r.ValidToDate)
                .Empty();
        });

        RuleFor(r => r.Status).IsInEnum();

        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}

