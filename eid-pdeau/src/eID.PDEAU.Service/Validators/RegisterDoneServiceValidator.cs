using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class RegisterDoneServiceValidator : AbstractValidator<RegisterDoneService>
{
    public RegisterDoneServiceValidator()
    {
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.ServiceName).NotEmpty().MaximumLength(1024);
        RuleFor(r => r.Count).GreaterThanOrEqualTo(1);
    }
}
