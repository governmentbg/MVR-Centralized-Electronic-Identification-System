using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class GetEmpowermentByIdValidator : AbstractValidator<GetEmpowermentById>
{
    public GetEmpowermentByIdValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.EmpowermentId).NotEmpty();
    }
}
