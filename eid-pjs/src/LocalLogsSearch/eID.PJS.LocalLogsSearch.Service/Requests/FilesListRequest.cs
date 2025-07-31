using FluentValidation;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service
{
    /// <summary>
    /// Encapsulates the request to filter the audit log files based on the information located in the name of the file and file extension.
    /// </summary>
    /// <seealso cref="eID.PJS.LocalLogsSearch.Service.IValidatableRequest" />
    public class FilesListRequest : IValidatableRequest
    {
        /// <summary>
        /// List of folders where to look for the log files
        /// </summary>
        public string[] Folders { get; set; }

        /// <summary>
        /// The extension of the log files to look for
        /// </summary>
        public string FileExtensionFilter { get; set; }

        /// <summary>
        /// Date range to filter on
        /// </summary>
        public DateRange FileDateRangeFilter { get; set; }

        /// <summary>
        /// Gets the assigned validator to validate the request
        /// </summary>
        /// <returns>Instance of SearchLogsRequestValidator</returns>
        public virtual IValidator GetValidator() => new FilesFilterValidator();
    }

    /// <summary>
    /// Validator for the FilesFilterValidator
    /// </summary>
    public class FilesFilterValidator : AbstractValidator<FilesListRequest>
    {
        /// <summary>
        /// SearchLogsRequestValidator empty constructor
        /// </summary>
        public FilesFilterValidator()
        {
            RuleFor(r => r.Folders).NotEmpty();
            RuleFor(r => r.FileDateRangeFilter).NotNull();
            RuleFor(r => r.FileDateRangeFilter).Must(m =>
            {
                return !DateRange.IsEmpty(m);
            });
            RuleFor(r => r.FileDateRangeFilter).Must(m =>
            {
                return m.FromDate > DateTime.MinValue && m.ToDate > DateTime.MinValue;
            }).WithMessage("FromDate and ToDate must be greater than minimal date: 0001-01-01T00:00:00");
            RuleFor(r => r.FileDateRangeFilter).Must(m =>
            {
                return m.FromDate < m.ToDate;
            }).WithMessage("ToDate must be after FromDate");
        }
    }
}
