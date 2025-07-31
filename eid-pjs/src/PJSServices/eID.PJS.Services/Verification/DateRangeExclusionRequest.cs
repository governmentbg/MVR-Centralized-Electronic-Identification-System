using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Verification;

using FluentValidation;

namespace eID.PJS.Services.Verification
{
    public class DateRangeExclusionRequest: IValidatableRequest
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
        /// Brief explanation why is it being excluded from the process
        /// </summary>
        public string ReasonForExclusion { get; set; }

        public virtual IValidator GetValidator() => new DateRangeExclusionRequestValidator();
    }
}

public class DateRangeExclusionRequestValidator : AbstractValidator<DateRangeExclusionRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateRangeExclusionValidator"/> class.
    /// </summary>
    public DateRangeExclusionRequestValidator() : base()
    {
        RuleFor(r => r.StartDate).NotEmpty();
        RuleFor(r => r.EndDate).NotEmpty();
        RuleFor(r => r.StartDate < r.EndDate);
        RuleFor(r => r.ReasonForExclusion).NotEmpty().MaximumLength(256);
    }
}
