using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetSmtpConfigurationsByFilterValidator : AbstractValidator<GetSmtpConfigurationsByFilter>
{
    public GetSmtpConfigurationsByFilterValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}
