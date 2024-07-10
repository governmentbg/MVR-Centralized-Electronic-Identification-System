using eID.RO.Contracts.Commands;
using FluentValidation;

namespace eID.RO.Service.Validators;

internal class GetActivatedEmpowermentsByYearValidator : AbstractValidator<GetActivatedEmpowermentsByYear>
{
    public GetActivatedEmpowermentsByYearValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().GreaterThan(0).LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
