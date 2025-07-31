using eID.PJS.Contracts;
using eID.PJS.Contracts.Commands.Admin;
using FluentValidation;

namespace eID.PJS.Service.Admin.Validators;

internal class GetLogFromUserAsFileValidator : AbstractValidator<GetLogFromUserAsFile>
{
    public GetLogFromUserAsFileValidator()
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
            .Must(date => date <= DateTime.UtcNow)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r.EndDate)
            .Must(date => date <= DateTime.UtcNow.AddDays(1).Date)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r)
            .Must(r => r.StartDate <= r.EndDate)
            .WithMessage($"{nameof(GetLogFromUserAsFile.StartDate)} must be less than or equal to {nameof(GetLogFromUserAsFile.EndDate)}.");

        RuleFor(r => r)
            .Must(r => r.StartDate.AddDays(Constants.FileProcess.MaxTimeSpanInDays) >= r.EndDate)
            .WithMessage($"Maximum {Constants.FileProcess.MaxTimeSpanInDays} days are allowed.")
            .WithName($"{nameof(GetLogFromUserAsFile.StartDate)} and {nameof(GetLogFromUserAsFile.EndDate)}");

        RuleFor(r => r.EventTypes)
            .Must(eventTypes => eventTypes == null || eventTypes.Any() && eventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot be empty or contain empty strings.");

        RuleFor(r => r.FileFullPath)
                .NotEmpty();
    }
}
