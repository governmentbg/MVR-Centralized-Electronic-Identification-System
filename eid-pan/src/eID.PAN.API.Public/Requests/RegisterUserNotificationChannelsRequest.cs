using FluentValidation;

namespace eID.PAN.API.Public.Requests;

public class RegisterUserNotificationChannelsRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterUserNotificationChannelsRequestValidator();
    public HashSet<Guid> Ids { get; set; }
}

internal class RegisterUserNotificationChannelsRequestValidator : AbstractValidator<RegisterUserNotificationChannelsRequest>
{
    public RegisterUserNotificationChannelsRequestValidator()
    {
        RuleFor(r => r.Ids).NotNull()
            .ForEach(id => id.NotEmpty());
    }
}
