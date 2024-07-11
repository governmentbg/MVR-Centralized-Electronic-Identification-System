using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class ApproveEmpowermentByDeauValidator : AbstractValidator<ApproveEmpowermentByDeau>
{
    public ApproveEmpowermentByDeauValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty();
        RuleFor(x => x.EmpowermentId).NotEmpty();
    }
}
