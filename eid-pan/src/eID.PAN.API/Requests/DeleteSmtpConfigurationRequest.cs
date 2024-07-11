using FluentValidation;

namespace eID.PAN.API.Requests;

public class DeleteSmtpConfigurationRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new DeleteSmtpConfigurationRequestValidator();

    public string? Id { get; set; }
}


public class DeleteSmtpConfigurationRequestValidator : AbstractValidator<DeleteSmtpConfigurationRequest>
{
    public DeleteSmtpConfigurationRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
