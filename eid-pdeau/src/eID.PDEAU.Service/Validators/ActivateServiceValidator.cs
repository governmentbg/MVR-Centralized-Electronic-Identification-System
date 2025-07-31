using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class ActivateServiceValidator : AbstractValidator<ActivateService>
{
    public ActivateServiceValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.ServiceId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");

        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
    }
}
