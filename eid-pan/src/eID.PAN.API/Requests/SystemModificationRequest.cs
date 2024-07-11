using FluentValidation;

namespace eID.PAN.API.Requests;

public class SystemModificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SystemModificationRequestValidator();
    public bool? IsApproved { get; set; }
    public bool? IsDeleted { get; set; }
}

internal class SystemModificationRequestValidator : AbstractValidator<SystemModificationRequest>
{
    public SystemModificationRequestValidator()
    {
        // IsApproved can be only true
        RuleFor(r => r.IsApproved)
            .Equal(true)
            .When(r => !r.IsDeleted.HasValue);

        RuleFor(r => r.IsDeleted)
            .NotNull()
            .When(r => !r.IsApproved.HasValue);

        // Only one of fields must have a value
        RuleFor(r => r)
            .Must(r => (!r.IsApproved.HasValue && r.IsDeleted.HasValue) || (r.IsApproved.HasValue && !r.IsDeleted.HasValue));
    }
}
