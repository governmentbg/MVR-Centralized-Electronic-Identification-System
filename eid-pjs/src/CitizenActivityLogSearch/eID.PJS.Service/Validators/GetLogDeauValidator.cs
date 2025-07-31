using eID.PJS.Contracts.Commands;
using FluentValidation;

namespace eID.PJS.Service.Validators;

internal class GetLogDeauValidator : AbstractValidator<GetLogDeau>
{
    public GetLogDeauValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.SystemId);

        RuleFor(r => r.CursorSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}
