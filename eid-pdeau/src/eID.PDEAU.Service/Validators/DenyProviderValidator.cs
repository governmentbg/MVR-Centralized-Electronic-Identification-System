using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Service.Entities;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class DenyProviderValidator : AbstractValidator<DenyProvider>
{
    public DenyProviderValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();
        RuleFor(r => r.ProviderId)
            .NotEmpty();

        RuleFor(r => r.IssuerUid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.IssuerUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.IssuerUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.IssuerUidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.IssuerUidType)
            .NotEmpty()
            .IsInEnum();

        RuleFor(r => r.IssuerName)
            .NotEmpty()
            .MaximumLength(DBConstraints.ProviderStatusHistory.ModifierNameMaxLength);

        RuleFor(r => r.Comment)
            .NotEmpty()
            .MaximumLength(Constants.ProviderStatusHistory.CommentMaxLength);
    }
}
