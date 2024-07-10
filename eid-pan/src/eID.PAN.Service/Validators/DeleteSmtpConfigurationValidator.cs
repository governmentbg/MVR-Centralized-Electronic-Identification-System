using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class DeleteSmtpConfigurationValidator : AbstractValidator<DeleteSmtpConfiguration>
{
    public DeleteSmtpConfigurationValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
