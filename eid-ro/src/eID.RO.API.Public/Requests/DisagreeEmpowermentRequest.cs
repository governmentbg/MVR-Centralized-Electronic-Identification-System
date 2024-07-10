using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class DisagreeEmpowermentRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new DisagreeEmpowermentRequestValidator();

    /// <summary>
    /// User EGN or LNCh got from token
    /// </summary>
    public string? Uid { get; set; }

    /// <summary>
    /// Uid type EGN or LNCh got from token
    /// </summary>
    public IdentifierType UidType { get; set; }

    /// <summary>
    /// Empowerment Id
    /// </summary>
    public Guid EmpowermentId { get; set; }

    /// <summary>
    /// Disagreement reason
    /// </summary>
    public string? Reason { get; set; }
}

internal class DisagreeEmpowermentRequestValidator : AbstractValidator<DisagreeEmpowermentRequest>
{
    public DisagreeEmpowermentRequestValidator()
    {
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

/// <summary>
/// All user-provided necessary data for empowerment disagreement process
/// </summary>
public class DisagreeEmpowermentRequestPayload
{
    /// <summary>
    /// The reason for the initialization of the disagreement process
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
