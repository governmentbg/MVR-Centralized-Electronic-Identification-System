using eID.MIS.Contracts.SEV.Commands;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class GetDeliveredMessagesByYearValidator : AbstractValidator<GetDeliveredMessagesByYear>
{
    public GetDeliveredMessagesByYearValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().GreaterThan(0).LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
