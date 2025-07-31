using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetAvailableScopesByProviderIdValidator : AbstractValidator<GetAvailableScopesByProviderId>
{
    public GetAvailableScopesByProviderIdValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}
