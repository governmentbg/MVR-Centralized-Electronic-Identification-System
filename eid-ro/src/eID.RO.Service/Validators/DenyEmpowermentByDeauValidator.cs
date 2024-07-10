using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class DenyEmpowermentByDeauValidator : AbstractValidator<DenyEmpowermentByDeau>
{
    public DenyEmpowermentByDeauValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty();
        RuleFor(x => x.EmpowermentId).NotEmpty();
        RuleFor(x => x.DenialReasonComment).NotEmpty();
    }
}
