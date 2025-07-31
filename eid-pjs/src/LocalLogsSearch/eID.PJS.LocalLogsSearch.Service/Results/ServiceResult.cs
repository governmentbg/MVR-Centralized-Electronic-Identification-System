using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation.Results;

namespace eID.PJS.LocalLogsSearch.Service
{
    public abstract class ServiceResult : IPerformanceMetrics
    {
        /// <summary>Gets or sets the general errors.</summary>
        /// <value>The errors.</value>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>Gets or sets the validation errors.</summary>
        /// <value>The validation errors.</value>
        public ValidationResult ValidationErrors { get; set; } = new ValidationResult();
        /// <summary>Gets or sets the performance metrics.</summary>
        /// <value>The metrics.</value>
        public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();
    }
}
