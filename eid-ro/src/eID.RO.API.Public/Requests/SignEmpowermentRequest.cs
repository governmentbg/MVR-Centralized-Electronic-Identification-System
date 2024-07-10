using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class SignEmpowermentRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SignEmpowermentRequestValidator();

    /// <summary>
    /// User name got from token
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// User EGN or LNCh got from token
    /// </summary>
    public string? Uid { get; set; }

    /// <summary>
    /// Uid type
    /// </summary>
    public IdentifierType UidType { get; set; }

    /// <summary>
    /// Empowerment Id
    /// </summary>
    public Guid EmpowermentId { get; set; }
}
internal class SignEmpowermentRequestValidator : AbstractValidator<SignEmpowermentRequest>
{
    public SignEmpowermentRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
        
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();

        RuleFor(r => r.EmpowermentId).NotEmpty();
    }
}

/// <summary>
/// All user-provided necessary data for empowerment signing
/// </summary>
public class SignEmpowermentPayload : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SignEmpowermentPayloadValidator();
    public SignatureProvider SignatureProvider { get; set; }
    /// <summary>
    /// Base64 encoded detached signature
    /// </summary>
    public string DetachedSignature { get; set; }
}

public class SignEmpowermentPayloadValidator : AbstractValidator<SignEmpowermentPayload>
{
    public SignEmpowermentPayloadValidator()
    {
        RuleFor(r => r.SignatureProvider)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum();

        RuleFor(r => r.DetachedSignature)
            .NotEmpty()
            .MaximumLength(65535); // Should resolve in 32kb of text
    }
}
