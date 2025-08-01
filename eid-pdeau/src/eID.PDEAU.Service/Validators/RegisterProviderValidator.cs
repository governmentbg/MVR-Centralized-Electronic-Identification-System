using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class RegisterProviderValidator : AbstractValidator<RegisterProvider>
{
    public RegisterProviderValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        // This rule is only for MoI administrators
        RuleFor(r => r.ExternalNumber)
            .NotEmpty()
            .MaximumLength(32)
            .When(r => !string.IsNullOrEmpty(r.CreatedByAdministratorId));
        
        RuleFor(r => r.ProviderId).NotEmpty();
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
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Type).IsInEnum().NotEmpty();

        RuleFor(r => r.Bulstat)
                .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid Eik.");

        RuleFor(r => r.Headquarters).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Address).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(200);

        RuleFor(r => r.Administrator).NotEmpty().SetValidator(new RegisterProviderUserValidator());

        RuleFor(r => r.FilesInformation).NotEmpty().SetValidator(new FilesInformationValidator(true));
    }
}

internal class FilesInformationValidator : AbstractValidator<FilesInformation>
{
    public FilesInformationValidator(bool filesAreMandatory)
    {
        RuleFor(r => r.UploaderUid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UploaderUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.UploaderUidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UploaderUidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UploaderUidType).NotEmpty().IsInEnum();

        RuleFor(r => r.UploaderName).NotEmpty().MaximumLength(200);

        if (filesAreMandatory)
        {
            RuleFor(r => r.Files).NotEmpty();
        }
        RuleForEach(r => r.Files)
            .SetValidator(new FileDataValidator());
    }
}

internal class FileDataValidator : AbstractValidator<FileData>
{
    public FileDataValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.FileName).NotEmpty();

        RuleFor(r => r.FullFilePath).NotEmpty()
            .Must(r => File.Exists(r))
            .WithMessage("File {PropertyValue} does not exist.");
    }
}

public class RegisterProviderUserValidator : AbstractValidator<RegisterProviderUser>
{
    public RegisterProviderUserValidator()
    {
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
        RuleFor(r => r.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(200);
    }
}

