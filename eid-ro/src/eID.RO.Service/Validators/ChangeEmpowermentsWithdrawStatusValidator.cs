using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class ChangeEmpowermentsWithdrawStatusValidator : AbstractValidator<ChangeEmpowermentWithdrawalStatus>
{
    public ChangeEmpowermentsWithdrawStatusValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EmpowermentWithdrawalId).NotEmpty();
        RuleFor(r => r.Status).IsInEnum();
    }
}
