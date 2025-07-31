using FluentValidation;

namespace eID.PAN.API.Requests;

public class EventModificationPayload
{
    public bool IsDeleted { get; set; }
}

public class EventModificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new EventModificationRequestValidator();

    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}

internal class EventModificationRequestValidator : AbstractValidator<EventModificationRequest>
{
    public EventModificationRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
