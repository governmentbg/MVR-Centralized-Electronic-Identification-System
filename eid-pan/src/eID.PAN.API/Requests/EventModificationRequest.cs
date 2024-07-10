using FluentValidation;

namespace eID.PAN.API.Requests;

public class EventModificationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new EventModificationRequestValidator();
    public bool IsDeleted { get; set; }
}

internal class EventModificationRequestValidator : AbstractValidator<EventModificationRequest>
{
    public EventModificationRequestValidator()
    {
    }
}
