using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class DenyServiceValidator : AbstractValidator<DenyService>
{
    public DenyServiceValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ServiceId).NotEmpty();
        RuleFor(r => r.DenialReason).NotEmpty().MaximumLength(Contracts.Constants.ProviderService.DenialReasonMaxLength);
    }
}
