using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetPersonalIdentityQuery : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetPersonalIdentityQueryValidator();

    /// <summary>
    /// Номер на документ за самоличност
    /// </summary>
    public string IdentityDocumentNumber { get; set; }

    /// <summary>
    /// ЕГН
    /// </summary>
    public string EGN { get; set; }
}

internal class GetPersonalIdentityQueryValidator : AbstractValidator<GetPersonalIdentityQuery>
{
    public GetPersonalIdentityQueryValidator()
    {
        RuleFor(r => r.IdentityDocumentNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.EGN)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(egn => ValidatorHelpers.EgnFormatIsValid(egn))
                .WithMessage("{PropertyName} is invalid.");
    }
}
