using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Requests;

public class UidAndTypeValidator : AbstractValidator<UidAndUidType>
{
    public UidAndTypeValidator()
    {
        When(r => r.UidType == IdentifierType.EGN, () => {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.UidType == IdentifierType.LNCh, () => {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });
    }
}
