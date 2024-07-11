using FluentValidation;

namespace eID.PAN.API.Requests;

public class RestoreSystemRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RestoreSystemRequestValidator();
    public Guid SystemId { get; set; }
}

public class RestoreSystemRequestValidator : AbstractValidator<RestoreSystemRequest>
{
    public RestoreSystemRequestValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
