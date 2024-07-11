using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class WithdrawEmpowermentRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new WithdrawEmpowermentRequestValidator();

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

    /// <summary>
    /// Withdraw reason
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string? Name { get; set; }
}

internal class WithdrawEmpowermentRequestValidator : AbstractValidator<WithdrawEmpowermentRequest>
{
    public WithdrawEmpowermentRequestValidator()
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

        RuleFor(r => r.Name).NotEmpty();
    }
}

/// <summary>
/// All user-provided necessary data for empowerment withdrawal process
/// </summary>
public class WithdrawEmpowermentRequestPayload
{
    /// <summary>
    /// The reason for the initialization of the withdrawal process
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
