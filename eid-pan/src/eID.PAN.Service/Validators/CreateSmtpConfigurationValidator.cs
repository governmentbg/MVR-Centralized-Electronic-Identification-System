using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class CreateSmtpConfigurationValidator : AbstractValidator<CreateSmtpConfiguration>
{
    public CreateSmtpConfigurationValidator()
    {
        RuleFor(r => r.Server).NotEmpty();
        RuleFor(r => r.Port).GreaterThan(0).LessThanOrEqualTo(65535);
        RuleFor(r => r.UserName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Password).NotEmpty();
        RuleFor(r => r.SecurityProtocol).IsInEnum();
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.SecurityProtocol).IsInEnum();
    }
}
