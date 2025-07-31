using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetProviderOfficesValidator : AbstractValidator<GetProviderOffices>
{
    public GetProviderOfficesValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
    }
}
