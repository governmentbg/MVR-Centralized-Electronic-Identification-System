using FluentValidation;

namespace eID.PAN.API.Requests;

public class ApproveSystemRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ApproveSystemRequestValidator();
    public Guid SystemId { get; set; }
}

public class ApproveSystemRequestValidator : AbstractValidator<ApproveSystemRequest>
{
    public ApproveSystemRequestValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
