using FluentValidation;
using eID.Signing.Service.Validators;

namespace eID.Signing.API.Public.Requests;

public class UserCheckRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new UserCheckRequestValidator();

    public string Uid { get; set; }
}

public class UserCheckRequestValidator : AbstractValidator<UserCheckRequest>
{
    public UserCheckRequestValidator()
    {
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
    }
}
