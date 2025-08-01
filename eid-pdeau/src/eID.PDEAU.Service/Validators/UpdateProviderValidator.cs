using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class UpdateProviderValidator : AbstractValidator<UpdateProvider>
{
    public UpdateProviderValidator()
    {
        RuleFor(r => r.ProviderId)
            .NotEmpty();
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.IssuerUid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.IssuerUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.IssuerUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.IssuerUidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.IssuerUidType).NotEmpty().IsInEnum();
        RuleFor(r => r.IssuerName).NotEmpty().MaximumLength(200);

        RuleFor(r => r.Comment)
            .NotEmpty()
            .MaximumLength(Constants.ProviderStatusHistory.CommentMaxLength);

        RuleFor(r => r.FilesInformation).NotEmpty().SetValidator(new FilesInformationValidator(false));
    }
}
