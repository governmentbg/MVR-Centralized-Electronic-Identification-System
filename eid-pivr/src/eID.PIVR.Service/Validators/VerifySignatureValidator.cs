using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

internal class VerifySignatureValidator : AbstractValidator<VerifySignature>
{
    public VerifySignatureValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();

        RuleFor(r => r.OriginalFile)
            .NotEmpty()
            .MaximumLength(524_287); // Should resolve in 512 kb of file

        RuleFor(r => r.DetachedSignature)
            .NotEmpty()
            .MaximumLength(1_048_575); // Should resolve in 512 kb of text

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");

        RuleFor(r => r.SignatureProvider)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum();

        RuleFor(r => r.UidType)
           .NotEmpty()
           .IsInEnum();
    }
}
