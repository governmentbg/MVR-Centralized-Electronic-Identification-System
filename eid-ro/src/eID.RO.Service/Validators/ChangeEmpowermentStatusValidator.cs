using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class ChangeEmpowermentStatusValidator : AbstractValidator<ChangeEmpowermentStatus>
{
    public ChangeEmpowermentStatusValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EmpowermentId).NotEmpty();
        RuleFor(r => r.Status).IsInEnum();

        When(x => x.Status == EmpowermentStatementStatus.Denied, () =>
        {
            RuleFor(r => r.DenialReason).NotEmpty().IsInEnum();
        });
    }
}
