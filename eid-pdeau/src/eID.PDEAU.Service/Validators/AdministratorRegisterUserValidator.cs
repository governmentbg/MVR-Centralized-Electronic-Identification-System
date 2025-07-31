using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class AdministratorRegisterUserValidator : AbstractValidator<AdministratorRegisterUser>
{
    public AdministratorRegisterUserValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();

        RuleFor(r => r.AdministratorUid)
            .NotEmpty()
            .MaximumLength(Constants.AdministratorAction.AdministratorUidMaxLength)
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.AdministratorUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.AdministratorUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.AdministratorUidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.AdministratorUidType).NotEmpty().IsInEnum();
        RuleFor(r => r.AdministratorFullName).NotEmpty().MaximumLength(Constants.AdministratorAction.AdministratorFullNameMaxLength);
        RuleFor(r => r.Comment).NotEmpty().MaximumLength(Constants.AdministratorAction.CommentMaxLength);

        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType).NotEmpty().IsInEnum();

        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Email).MaximumLength(200).EmailAddress();
        RuleFor(r => r.Phone).MaximumLength(200);
    }
}
