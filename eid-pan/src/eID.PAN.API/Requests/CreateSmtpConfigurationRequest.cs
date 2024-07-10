using eID.PAN.Contracts.Enums;
using FluentValidation;

namespace eID.PAN.API.Requests;

public class CreateSmtpConfigurationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new CreateSmtpConfigurationRequestValidator();

    public string Server { get; set; }
    public int Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class CreateSmtpConfigurationRequestValidator : AbstractValidator<CreateSmtpConfigurationRequest>
{
    public CreateSmtpConfigurationRequestValidator()
    {
        RuleFor(r => r.Server).NotEmpty();
        RuleFor(r => r.Port).GreaterThan(0).LessThanOrEqualTo(65535);
        RuleFor(r => r.UserName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Password).NotEmpty();
        RuleFor(r => r.SecurityProtocol).IsInEnum();
    }
}
