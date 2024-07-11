using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

internal class GetUserNotificationChannelsByFilterValidator : AbstractValidator<GetUserNotificationChannelsByFilter>
{
    public GetUserNotificationChannelsByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}
