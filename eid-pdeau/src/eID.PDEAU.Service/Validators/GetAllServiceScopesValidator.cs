using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetAllServiceScopesValidator : AbstractValidator<GetAllServiceScopes>
{
    public GetAllServiceScopesValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ServiceId).NotEmpty();
    }
}
