using eID.PJS.Contracts;
using eID.PJS.Contracts.Commands;
using FluentValidation;

namespace eID.PJS.Service.Validators;

internal class GetLogUserFromMeValidator : AbstractValidator<GetLogUserFromMe>
{
    public GetLogUserFromMeValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.StartDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r.EndDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddDays(1).Date)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r)
            .Must(r => !r.StartDate.HasValue || !r.EndDate.HasValue || r.StartDate <= r.EndDate)
            .WithMessage($"{nameof(GetLogUserFromMe.StartDate)} must be less than or equal to {nameof(GetLogUserFromMe.EndDate)}.");

        RuleFor(r => r.EventTypes)
            .Must(eventTypes => eventTypes == null || eventTypes.Any() && eventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot be empty or contain empty strings.");

        RuleFor(r => r.ExcludedEventTypes)
            .Must(excludedEventTypes => excludedEventTypes == null || !excludedEventTypes.Any() || excludedEventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot contain empty strings.");

        RuleFor(r => r.CursorSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(Constants.CursorSize.UserFormToMax)
            .Unless(r => r.CursorSize == 1000 && HasActiveFilter(r));
    }
    private bool HasActiveFilter(GetLogUserFromMe r)
    {
        return (r.EventTypes != null && r.EventTypes.Any())
            || (r.ExcludedEventTypes != null && r.ExcludedEventTypes.Any())
            || r.StartDate.HasValue
            || r.EndDate.HasValue;
    }
}
