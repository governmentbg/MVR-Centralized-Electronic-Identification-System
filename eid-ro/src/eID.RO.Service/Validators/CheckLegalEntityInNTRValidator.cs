using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class CheckLegalEntityInNTRValidator : AbstractValidator<CheckLegalEntityInNTR>
{
    public CheckLegalEntityInNTRValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.IssuerUid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                    .WithMessage("{PropertyName} contains invalid records.");
        RuleFor(r => r.IssuerUidType).NotEmpty().IsInEnum();

        RuleFor(r => r.IssuerName).NotEmpty();
        RuleFor(r => r.IssuerPosition).NotEmpty();

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(r => ValidatorHelpers.EikFormatIsValid(r));

        RuleFor(r => r.Name).NotEmpty();
    }
}
