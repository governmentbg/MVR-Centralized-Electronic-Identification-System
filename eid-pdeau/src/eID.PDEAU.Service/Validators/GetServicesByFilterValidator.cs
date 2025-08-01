using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetServicesByFilterValidator : AbstractValidator<GetServicesByFilter>
{
    public GetServicesByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.Name).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.Name), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.Description).MaximumLength(512)
            .When(r => !string.IsNullOrEmpty(r.Description), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.FindServiceNumberAndName).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.FindServiceNumberAndName), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).IsInEnum();
        });
    }
}
