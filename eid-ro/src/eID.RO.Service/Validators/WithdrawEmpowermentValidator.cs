using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class WithdrawEmpowermentValidator : AbstractValidator<WithdrawEmpowerment>
{
    public WithdrawEmpowermentValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
        RuleFor(r => r.EmpowermentId).NotEmpty();
        When(r => !string.IsNullOrWhiteSpace(r.Reason), () =>
        {
            RuleFor(r => r.Reason)
                .NotEmpty()
                .MaximumLength(256);
        });
    }
}
