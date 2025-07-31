using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.Service.Validators;

public class BoricaCheckUserByUidValidator : AbstractValidator<BoricaCheckUserByUid>
{
    public BoricaCheckUserByUidValidator()
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
