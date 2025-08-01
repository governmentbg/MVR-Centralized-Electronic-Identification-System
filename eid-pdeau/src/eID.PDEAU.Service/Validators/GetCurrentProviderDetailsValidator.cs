using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetCurrentProviderDetailsValidator : AbstractValidator<GetCurrentProviderDetails>
{
    public GetCurrentProviderDetailsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
    }
}
