using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class SetProviderDetailsStatusValidator : AbstractValidator<SetProviderDetailsStatus>
{
    public SetProviderDetailsStatusValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Status).IsInEnum().NotEmpty();
    }
}
