using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class GetLegalEntityInfoRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetLegalEntityInfoRequestValidator();

    /// <summary>
    /// ЕИК
    /// </summary>
    public string UIC { get; set; }
}


internal class GetLegalEntityInfoRequestValidator : AbstractValidator<GetLegalEntityInfoRequest>
{
    public GetLegalEntityInfoRequestValidator()
    {
        RuleFor(r => r.UIC)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => ValidatorHelpers.EikFormatIsValid(x))
                .WithMessage("{PropertyName} is invalid.");
    }
}
