using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

internal class RejectNotificationChannelValidator : AbstractValidator<RejectNotificationChannel>
{
    public RejectNotificationChannelValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.ModifiedBy)
            .NotEmpty();

        RuleFor(r => r.Reason)
            .NotEmpty()
            .MaximumLength(FieldLength.NotificationChannel.RejectReason);
    }
}
