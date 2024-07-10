using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentsByEikValidator : AbstractValidator<GetEmpowermentsByEik>
{
    public GetEmpowermentsByEikValidator()
    {
        RuleFor(r => r.Eik)
             .NotEmpty()
             .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                 .WithMessage("{PropertyName} invalid Eik.");

        RuleFor(x => x.IssuerUid).NotEmpty();
        RuleFor(r => r.IssuerUidType).NotEmpty().IsInEnum();

        When(r => r.IssuerUidType == IdentifierType.EGN, () => {
            RuleFor(r => r.IssuerUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.IssuerUidType == IdentifierType.LNCh, () => {
            RuleFor(r => r.IssuerUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });

        When(r => r.AuthorizerUids != null, () =>
        {
            RuleFor(r => r.AuthorizerUids)
                .NotEmpty()
                .ForEach(r => r.SetValidator(new UserIdentifierValidator()));
        });

        When(r => r.ShowOnlyNoExpiryDate.HasValue && r.ShowOnlyNoExpiryDate == true, () =>
        {
            RuleFor(r => r.ValidToDate)
                .Empty();
        });

        RuleFor(r => r.Status).IsInEnum();

        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
