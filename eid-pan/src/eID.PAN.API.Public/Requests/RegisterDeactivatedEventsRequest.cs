using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class RegisterDeactivatedEventsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterDeactivatedEventsRequestValidator();
    public HashSet<Guid> Ids { get; set; }
}

internal class RegisterDeactivatedEventsRequestValidator : AbstractValidator<RegisterDeactivatedEventsRequest>
{
    public RegisterDeactivatedEventsRequestValidator()
    {
        RuleFor(r => r.Ids).NotNull()
            .ForEach(id => id.NotEmpty());
    }
}
