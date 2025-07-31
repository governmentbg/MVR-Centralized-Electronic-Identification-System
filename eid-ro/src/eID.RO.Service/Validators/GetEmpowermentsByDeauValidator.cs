using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentsByDeauValidator : AbstractValidator<GetEmpowermentsByDeau>
{
    public GetEmpowermentsByDeauValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty();

        //RuleFor(x => x.OnBehalfOf).NotEmpty().IsInEnum();
        When(x => x.OnBehalfOf == OnBehalfOf.LegalEntity, () =>
        {
            // We validate Uid of LegalEntities for Uid as well because there are cases where it can be used as Bulstat. Examples: ЕТ, свободни професии и др.
            RuleFor(x => x.AuthorizerUid).Must(x => ValidatorHelpers.EikFormatIsValid(x) || ValidatorHelpers.UidFormatIsValid(x))
                .WithMessage("AuthorizerUid contains invalid Eik.");
        });

        RuleFor(x => x.AuthorizerUid).NotEmpty();
        When(x => x.OnBehalfOf == OnBehalfOf.Individual, () =>
        {
            When(r => r.AuthorizerUidType == IdentifierType.EGN, () => {
                RuleFor(r => r.AuthorizerUid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                        .WithMessage("{PropertyName} invalid EGN.")
                    .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                    .WithMessage("{PropertyName} people below lawful age.");
            });

            When(r => r.AuthorizerUidType == IdentifierType.LNCh, () => {
                RuleFor(r => r.AuthorizerUid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid LNCh.");
            });
            RuleFor(r => r.AuthorizerUidType).NotEmpty().IsInEnum();
        });

        RuleFor(x => x.EmpoweredUid).NotEmpty();
        When(r => r.EmpoweredUidType == IdentifierType.EGN, () => {
            RuleFor(r => r.EmpoweredUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.EmpoweredUidType == IdentifierType.LNCh, () => {
            RuleFor(r => r.EmpoweredUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });
        RuleFor(r => r.EmpoweredUidType).NotEmpty().IsInEnum();

        When(x => x.VolumeOfRepresentation != null, () =>
        {
            RuleForEach(x => x.VolumeOfRepresentation).NotEmpty();
        });

        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.RequesterUid).NotEmpty().Must(x => ValidatorHelpers.UidFormatIsValid(x));

        RuleFor(x => x.StatusOn).NotEmpty();

        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).IsInEnum();
        });
    }
}
