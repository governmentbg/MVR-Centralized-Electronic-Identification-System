using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class SignEmpowermentValidator : AbstractValidator<SignEmpowerment>
{
    public SignEmpowermentValidator()
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
        RuleFor(r => r.SignatureProvider)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum();

        RuleFor(r => r.DetachedSignature)
            .NotEmpty()
            .MaximumLength(65535); // Should resolve in 32kb of text
    }
}
