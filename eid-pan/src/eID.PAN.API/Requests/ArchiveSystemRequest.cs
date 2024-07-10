using FluentValidation;

namespace eID.PAN.API.Requests;

public class ArchiveSystemRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ArchiveSystemRequestValidator();
    public Guid SystemId { get; set; }
}

public class ArchiveSystemRequestValidator : AbstractValidator<ArchiveSystemRequest>
{
    public ArchiveSystemRequestValidator()
    {
        RuleFor(r => r.SystemId).NotEmpty();
    }
}
