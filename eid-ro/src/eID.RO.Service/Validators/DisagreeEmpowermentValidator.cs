using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class DisagreeEmpowermentValidator : AbstractValidator<DisagreeEmpowerment>
{
    public DisagreeEmpowermentValidator()
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
        RuleFor(r => r.Reason).NotEmpty()
            .MaximumLength(256);
    }
}
