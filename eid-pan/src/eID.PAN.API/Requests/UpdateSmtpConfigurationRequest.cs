using eID.PAN.Contracts.Enums;
using FluentValidation;

namespace eID.PAN.API.Requests;
public class UpdateSmtpConfigurationPayload
{
    public string? Server { get; set; }
    public int Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public class UpdateSmtpConfigurationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new UpdateSmtpConfigurationRequestValidator();

    public string? Id { get; set; }
    public string? Server { get; set; }
    public int Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public class UpdateSmtpConfigurationRequestValidator : AbstractValidator<UpdateSmtpConfigurationRequest>
{
    public UpdateSmtpConfigurationRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Port).GreaterThan(0).LessThanOrEqualTo(65535);
        RuleFor(r => r.SecurityProtocol).IsInEnum();
    }
}
