using eID.PJS.Contracts;
using eID.PJS.Contracts.Commands.Admin;
using FluentValidation;

namespace eID.PJS.Service.Admin.Validators;

internal class GetLogFromUserValidator : AbstractValidator<GetLogFromUser>
{
    public GetLogFromUserValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        When(r => !string.IsNullOrWhiteSpace(r.TargetUid), () =>
        {
            RuleFor(r => r.TargetUidType).NotEmpty().IsInEnum();
        });

        When(r => !string.IsNullOrWhiteSpace(r.RequesterUid), () =>
        {
            RuleFor(r => r.RequesterUidType).NotEmpty().IsInEnum();
        });

        RuleFor(r => r.StartDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r.EndDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddDays(1).Date)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r)
            .Must(r => !r.StartDate.HasValue || !r.EndDate.HasValue || r.StartDate <= r.EndDate)
            .WithMessage($"{nameof(GetLogFromUser.StartDate)} must be less than or equal to {nameof(GetLogFromUser.EndDate)}.");

        RuleFor(r => r.EventTypes)
            .Must(eventTypes => eventTypes == null || eventTypes.Any() && eventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot be empty or contain empty strings.");

        RuleFor(r => r.CursorSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(Constants.CursorSize.UserFormToMax)
            .Unless(r => r.CursorSize == 1000 && HasActiveFilter(r));
    }
    private bool HasActiveFilter(GetLogFromUser r)
    {
        return (r.EventTypes != null && r.EventTypes.Any())
            || r.StartDate.HasValue
            || r.EndDate.HasValue;
    }
}
