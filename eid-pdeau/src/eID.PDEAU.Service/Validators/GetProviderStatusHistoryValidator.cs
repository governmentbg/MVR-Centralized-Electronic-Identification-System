using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class GetProviderStatusHistoryValidator : AbstractValidator<GetProviderStatusHistory>
{
    public GetProviderStatusHistoryValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}
