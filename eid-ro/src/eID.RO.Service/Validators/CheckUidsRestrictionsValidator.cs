using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class CheckUidsRestrictionsValidator : AbstractValidator<CheckUidsRestrictions>
{
    public CheckUidsRestrictionsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Uids)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new UserIdentifierWithNameWithoutLawfulAgeValidator()));

    }
}
internal class VerifyUidsLawfulAgeValidator : AbstractValidator<VerifyUidsLawfulAge>
{
    public VerifyUidsLawfulAgeValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Uids)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new UserIdentifierWithoutLawfulAgeValidator()));
    }
}
internal class UserIdentifierWithoutLawfulAgeValidator : AbstractValidator<UserIdentifier>
{
    public UserIdentifierWithoutLawfulAgeValidator()
    {
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType)
            .NotEmpty()
            .IsInEnum();
    }
}

internal class UserIdentifierWithNameWithoutLawfulAgeValidator : AbstractValidator<UserIdentifier>
{
    public UserIdentifierWithNameWithoutLawfulAgeValidator()
    {
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType)
            .NotEmpty()
            .IsInEnum();
    }
}
