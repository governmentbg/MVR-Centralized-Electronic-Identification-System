using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetProviderDetailsByFilterValidator : AbstractValidator<GetProviderDetailsByFilter>
{
    public GetProviderDetailsByFilterValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.Name).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.Name), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.Status).IsInEnum().WithMessage("{PropertyName} {PropertyValue} is not allowed.");
    }
}
