using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetActualStateByUICQuery : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetActualStateByUICQueryValidator();

    /// <summary>
    /// Can contain either 9 or 13 digits, or 10 digits that should resolve in valid, above legal age EGN.
    /// </summary>
    public string UIC { get; set; } = string.Empty;
    /// <summary>
    /// Must be comma-separated values. Use for reference: https://info-regix.egov.bg/public/administrations/-/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualStateV3
    /// </summary>
    public string? AdditionalFieldList { get; set; }
}

public class GetActualStateByUICQueryValidator : AbstractValidator<GetActualStateByUICQuery>
{
    public GetActualStateByUICQueryValidator()
    {
        RuleFor(r => r.UIC)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).WithMessage("{PropertyName} is not valid.").When(r => r.UIC?.Length == 10, ApplyConditionTo.CurrentValidator)
                .Must(r => r?.Length == 9 || r?.Length == 13).WithMessage("{PropertyName} is not valid.").When(r => r.UIC?.Length != 10, ApplyConditionTo.CurrentValidator);

    }
}


