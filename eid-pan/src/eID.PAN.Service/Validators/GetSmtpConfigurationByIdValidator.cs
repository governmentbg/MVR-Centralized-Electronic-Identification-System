using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetSmtpConfigurationByIdValidator : AbstractValidator<GetSmtpConfigurationById>
{
    public GetSmtpConfigurationByIdValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
