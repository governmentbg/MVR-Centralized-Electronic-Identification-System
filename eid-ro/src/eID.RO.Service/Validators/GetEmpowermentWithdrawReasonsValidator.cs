using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

public class GetEmpowermentWithdrawReasonsValidator : AbstractValidator<GetEmpowermentWithdrawReasons>
{
    public GetEmpowermentWithdrawReasonsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
    }
}
