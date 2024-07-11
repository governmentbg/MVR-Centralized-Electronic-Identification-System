using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetDeactivatedUserNotificationsValidator : AbstractValidator<GetDeactivatedUserNotifications>
{
    public GetDeactivatedUserNotificationsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
    }
}
