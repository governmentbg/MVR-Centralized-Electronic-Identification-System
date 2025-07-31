using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class TestNotificationChannelValidator : AbstractValidator<TestNotificationChannel>
{
    public TestNotificationChannelValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
    }
}

