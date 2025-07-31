using eID.PIVR.Contracts.Enums;
using FluentValidation;

namespace eID.PIVR.API.Requests;

public class VerifySignatureRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new VerifySignatureRequestValidator();

    /// <summary>
    /// Original file as text
    /// </summary>
    public string OriginalFile { get; set; } = string.Empty;

    /// <summary>
    /// Base64 encoded detached signature
    /// </summary>
    public string DetachedSignature { get; set; } = string.Empty;

    /// <summary>
    /// Citizen EGN or LNCh
    /// </summary>
    public string Uid { get; set; } = string.Empty;

    /// <summary>
    /// Uid type EGN or LNCh
    /// </summary>
    public IdentifierType UidType { get; set; }

    /// <summary>
    /// Certificate signature provider
    /// </summary>
    public SignatureProvider SignatureProvider { get; set; }
}

internal class VerifySignatureRequestValidator : AbstractValidator<VerifySignatureRequest>
{
    public VerifySignatureRequestValidator()
    {
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
