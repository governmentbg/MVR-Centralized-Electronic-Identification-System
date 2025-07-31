using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace eID.PJS.LocalLogsSearch.Service
{
    /// <summary>
    /// Class that incapsulates the filter for audit log files.
    /// It contains filters by the information in the file name such as date range , System ID and file extension
    /// It can provide also a Json query to filter the records inside the file.
    /// </summary>
    public class SearchLogsRequest : FilesListRequest, IValidatableRequest
    {
        /// <summary>
        /// Gets the assigned validator to validate the request
        /// </summary>
        /// <returns>Instance of SearchLogsRequestValidator</returns>
        public override IValidator GetValidator() => new SearchLogsFilterValidator();

        
        /// <summary>
        /// Query that can be executed over any JObject
        /// </summary>
        public QueryNode? DataQuery { get; set; }
    }

    /// <summary>
    /// Validator for the SearchLogsRequest
    /// </summary>
    public class SearchLogsFilterValidator : AbstractValidator<SearchLogsRequest>
    {
        /// <summary>
        /// SearchLogsRequestValidator empty constructor
        /// </summary>
        public SearchLogsFilterValidator()
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
