using FluentValidation;

#nullable disable

namespace eID.PJS.Services.Verification;

/// <summary>
/// Defines an exclusion based on date range.
/// </summary>
/// <seealso cref="VerificationExclusion" />
public class DateRangeExclusion : VerificationExclusion, IValidatableRequest
{
    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    /// <value>
    /// Start date.
    /// </value>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    /// <returns></returns>
    public override IValidator GetValidator() => new DateRangeExclusionValidator();

}

/// <summary>
/// DateRangeExclusion validator
/// </summary>
/// <seealso cref="AbstractValidator&lt;DateRangeExclusion&gt;" />
public class DateRangeExclusionValidator : ExclusionValidator<DateRangeExclusion>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateRangeExclusionValidator"/> class.
    /// </summary>
    public DateRangeExclusionValidator() : base()
    {
        RuleFor(r => r.StartDate).NotEmpty();
        RuleFor(r => r.EndDate).NotEmpty();
        RuleFor(r => r.StartDate < r.EndDate);
        RuleFor(r => r.ReasonForExclusion).NotEmpty().MaximumLength(256);
    }
}
