using eID.PAN.Contracts;
using FluentValidation;

namespace eID.PAN.API.Requests;

public class RejectNotificationChannelPayload
{
    public string Reason { get; set; }
}

public class RejectNotificationChannelRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RejectNotificationChannelRequestValidator();

    public Guid Id { get; set; }

    public string Reason { get; set; }
}

public class RejectNotificationChannelRequestValidator : AbstractValidator<RejectNotificationChannelRequest>
{
    public RejectNotificationChannelRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();

        RuleFor(r => r.Reason)
            .NotEmpty()
            .MaximumLength(FieldLength.NotificationChannel.RejectReason);
    }
}
