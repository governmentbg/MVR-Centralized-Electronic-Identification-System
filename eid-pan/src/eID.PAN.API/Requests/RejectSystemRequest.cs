using FluentValidation;

namespace eID.PAN.API.Requests;

public class RejectSystemRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RejectSystemRequestValidator();
    public Guid SystemId { get; set; }
}

public class RejectSystemRequestValidator : AbstractValidator<RejectSystemRequest>
{
    public RejectSystemRequestValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
