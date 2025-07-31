using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class SendChannelActivatedNotificationValidator : AbstractValidator<SendChannelActivatedNotification>
{
    public SendChannelActivatedNotificationValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.ChannelId).NotEmpty();
    }
}
