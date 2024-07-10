using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class UpdateSmtpConfigurationValidator : AbstractValidator<UpdateSmtpConfiguration>
{
    public UpdateSmtpConfigurationValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Port).GreaterThan(0).LessThanOrEqualTo(65535);
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.SecurityProtocol).IsInEnum();
    }
}
