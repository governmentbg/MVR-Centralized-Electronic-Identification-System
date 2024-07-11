using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class RegisterDeactivatedEventsValidator : AbstractValidator<RegisterDeactivatedEvents>
{
    public RegisterDeactivatedEventsValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.Ids)
            .NotNull();

        RuleFor(r => r.ModifiedBy)
            .NotEmpty();
    }
}
