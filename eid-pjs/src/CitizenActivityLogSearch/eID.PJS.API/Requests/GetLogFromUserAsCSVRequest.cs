using eID.PJS.Contracts;
using eID.PJS.Contracts.Commands.Admin;
using FluentValidation;

namespace eID.PJS.API.Requests;

public class GetLogFromUserAsCSVRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetLogFromUserAsCSVValidator();

    /// <summary>
    /// Filter for start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Filter for end date. Optional. Default: End of today (UTC)
    /// </summary>
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);

    /// <summary>
    /// Type or events to be obtained. Optional
    /// </summary>
    public string[]? EventTypes { get; set; }

    /// <summary>
    /// When querying for specific event
    /// </summary>
    public string? EventId { get; set; }
    /// <summary>
    /// Requester user id
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// User eidentity id taken from token
    /// </summary>
    public string? UserEid { get; set; }

    /// <summary>
    /// Uid of the person who made the request
    /// </summary>
    public string? RequesterUid { get; set; }
    public IdentifierType RequesterUidType { get; set; }
    /// <summary>
    /// Uid of the person that was subject of the action
    /// </summary>
    public string? TargetUid { get; set; }
    public IdentifierType TargetUidType { get; set; }
    /// <summary>
    /// Name of the person that was subject of the action
    /// </summary>
    public string? TargetName { get; set; }
}

internal class GetLogFromUserAsCSVValidator : AbstractValidator<GetLogFromUserAsCSVRequest>
{
    public GetLogFromUserAsCSVValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

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
            .WithName($"{nameof(GetLogFromUserAsCSVRequest.StartDate)} and {nameof(GetLogFromUserAsCSVRequest.EndDate)}");

        RuleFor(r => r.EventTypes)
            .Must(eventTypes => eventTypes == null || eventTypes.Any() && eventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot be empty or contain empty strings.");
    }
}
