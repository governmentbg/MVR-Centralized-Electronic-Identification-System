using eID.PJS.Contracts;
using FluentValidation;

namespace eID.PJS.API.Public.Requests;

public class GetLogUserToMeRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetLogUserToMeRequestValidator();

    /// <summary>
    /// Filter for start date. Optional.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter for end date. Optional.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Type or events to be obtained. Optional
    /// </summary>
    public string[]? EventTypes { get; set; }

    /// <summary>
    /// Size of cursor return data
    /// </summary>
    public int CursorSize { get; set; }

    /// <summary>
    /// The parameter provides a live cursor that uses the previous page’s results to obtain the next page’s results
    /// In the first request it will be null
    /// </summary>
    public IEnumerable<object>? CursorSearchAfter { get; set; }
}

internal class GetLogUserToMeRequestValidator : AbstractValidator<GetLogUserToMeRequest>
{
    public GetLogUserToMeRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.StartDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r.EndDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddDays(1).Date)
            .WithMessage("{PropertyName} cannot be in the future.");

        RuleFor(r => r)
            .Must(r => !r.StartDate.HasValue || !r.EndDate.HasValue || r.StartDate <= r.EndDate)
            .WithMessage($"{nameof(GetLogUserToMeRequest.StartDate)} must be less than or equal to {nameof(GetLogUserToMeRequest.EndDate)}.");

        RuleFor(r => r.EventTypes)
            .Must(eventTypes => eventTypes == null || eventTypes.Any() && eventTypes.All(eventType => !string.IsNullOrEmpty(eventType)))
            .WithMessage("{PropertyName} cannot be empty or contain empty strings.");

        RuleFor(r => r.CursorSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(Constants.CursorSize.UserFormToMax)
            .Unless(r => r.CursorSize == 1000 && HasActiveFilter(r));
    }
    private bool HasActiveFilter(GetLogUserToMeRequest r)
    {
        return (r.EventTypes != null && r.EventTypes.Any())
            || r.StartDate.HasValue
            || r.EndDate.HasValue;
    }
}
