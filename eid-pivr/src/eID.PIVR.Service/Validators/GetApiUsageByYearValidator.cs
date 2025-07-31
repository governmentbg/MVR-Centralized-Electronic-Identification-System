using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

internal class GetApiUsageByYearValidator : AbstractValidator<GetApiUsageByYear>
{
    public GetApiUsageByYearValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().GreaterThan(0).LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
