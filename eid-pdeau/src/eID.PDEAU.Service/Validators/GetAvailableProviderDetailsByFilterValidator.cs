using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetAvailableProviderDetailsByFilterValidator : AbstractValidator<GetAvailableProviderDetailsByFilter>
{
    public GetAvailableProviderDetailsByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
    }
}
