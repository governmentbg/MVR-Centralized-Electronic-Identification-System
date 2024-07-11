using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

internal class RegisterUserNotificationChannelsValidator : AbstractValidator<RegisterUserNotificationChannels>
{
    public RegisterUserNotificationChannelsValidator()
    {
        RuleFor(r => r.CorrelationId)
                    .NotEmpty();

        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.Ids)
            .NotNull()
            .ForEach(id => id.NotEmpty());

        RuleFor(r => r.ModifiedBy)
            .NotEmpty();
    }
}
