using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentDisagreementReasonsValidator : AbstractValidator<GetEmpowermentDisagreementReasons>
{
    public GetEmpowermentDisagreementReasonsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
    }
}
