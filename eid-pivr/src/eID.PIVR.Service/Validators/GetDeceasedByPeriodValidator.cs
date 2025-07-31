using eID.PIVR.Contracts.Commands;
using FluentValidation;

namespace eID.PIVR.Service.Validators;

internal class GetDeceasedByPeriodValidator : AbstractValidator<GetDeceasedByPeriod>
{
    public GetDeceasedByPeriodValidator()
    {
        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.From)
            .NotEmpty();

        RuleFor(r => r.To)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.From <= x.To)
                .WithMessage($"{nameof(GetDeceasedByPeriod.From)} must lower or equal than {nameof(GetDeceasedByPeriod.To)}");
    }
}
