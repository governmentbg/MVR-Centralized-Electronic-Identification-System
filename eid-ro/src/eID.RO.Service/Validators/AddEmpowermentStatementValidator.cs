using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.Service.Validators;
internal class AddEmpowermentStatementValidator : AbstractValidator<AddEmpowermentStatement>
{
    public AddEmpowermentStatementValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.OnBehalfOf).NotEmpty().IsInEnum();
        When(r => r.OnBehalfOf == OnBehalfOf.LegalEntity, () =>
        {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid Eik.");
            RuleFor(r => r.IssuerPosition).NotEmpty();
            RuleFor(r => r.UidType).Equal(IdentifierType.NotSpecified);
        });
        When(r => r.OnBehalfOf == OnBehalfOf.Individual, () =>
        {
            RuleFor(r => r.UidType).NotEmpty().IsInEnum();
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                    .When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                    .WithMessage("{PropertyName} person below lawful age.")
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                    .When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                    .WithMessage("{PropertyName} invalid Uid.");
        });
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Uid).NotEmpty();

        RuleFor(r => r.AuthorizerUids)
            .NotEmpty()
            .Must(q => HaveNoDuplicates(q))
                .WithMessage("{PropertyName} contains duplicates.")
            .ForEach(r => r.SetValidator(new UserIdentifierWithNameValidator()));
        RuleFor(r => r.EmpoweredUids)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
        RuleFor(r => r.TypeOfEmpowerment).IsInEnum();

        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.ProviderName).NotEmpty();
        RuleFor(r => r.ServiceName).NotEmpty();
        RuleFor(r => r.VolumeOfRepresentation)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new VolumeOfRepresentationValidator()));
        RuleFor(r => r.StartDate).NotEmpty();
        When(r => r.ExpiryDate.HasValue, () =>
        {
            RuleFor(r => r.ExpiryDate).Cascade(CascadeMode.Stop).GreaterThanOrEqualTo(r => r.StartDate);
        });
        RuleFor(r => r.CreatedBy).NotEmpty();

        When(r => !r.AllowSelfEmpowerment, () =>
        {
            When(r => r.OnBehalfOf == OnBehalfOf.Individual, () =>
            {
                RuleFor(r => r.AuthorizerUids.Any(au => r.EmpoweredUids.Any(eu => eu.Uid == au.Uid && eu.UidType == au.UidType)))
                    .NotEqual(true)
                    .WithName("AuthorizerUids, EmpoweredUids")
                    .WithMessage("Self empowerment is not allowed");
            });

            When(r => r.OnBehalfOf == OnBehalfOf.LegalEntity && r.AuthorizerUids.DistinctBy(au => (au.Uid, au.UidType)).Count() == 1, () =>
            {
                RuleFor(r => r.EmpoweredUids.Any(eu => eu.Uid == r.AuthorizerUids.First().Uid
                            && eu.UidType == r.AuthorizerUids.First().UidType))
                    .NotEqual(true)
                    .WithName("AuthorizerUids, EmpoweredUids")
                    .WithMessage("Self empowerment is not allowed");
            });
        });
    }
    private bool HaveNoDuplicates(IEnumerable<AuthorizerIdentifier> items)
    {
        if (items is null)
        {
            return true;
        }
        return items.Select(r => r.Uid).Distinct().Count() == items.Count();
    }
}

public class VolumeOfRepresentationValidator : AbstractValidator<VolumeOfRepresentationResult>
{
    public VolumeOfRepresentationValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
    }
}

public class UserIdentifierValidator : AbstractValidator<UserIdentifier>
{
    public UserIdentifierValidator()
    {
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} people below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType)
            .NotEmpty()
            .IsInEnum();
    }
}

public class UserIdentifierWithNameValidator : AbstractValidator<AuthorizerIdentifier>
{
    public UserIdentifierWithNameValidator()
    {
        When(r => r.UidType == IdentifierType.EGN, () =>
        {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.UidType == IdentifierType.LNCh, () =>
        {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });

        RuleFor(r => r.Name)
             .NotEmpty()
             .MaximumLength(200)
             .Must(name => ValidatorHelpers.UserIdentifierNameIsValid(name))
             .WithMessage("{PropertyName} containts invalid symbols. Only Bulgarian letters, dashes, apostrophes and space are allowed");
    }
}
