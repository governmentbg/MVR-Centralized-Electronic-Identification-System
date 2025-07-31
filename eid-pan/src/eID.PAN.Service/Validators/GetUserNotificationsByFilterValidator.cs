using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetUserNotificationsByFilterValidator : AbstractValidator<GetSystemsAndNotificationsByFilter>
{
    public GetUserNotificationsByFilterValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
