using FluentValidation.Results;

namespace eID.PJS.LocalLogsSearch.Service
{
    public class QueryEstimationResult : ServiceResult
    {
        /// <summary>Gets or sets the records count.</summary>
        /// <value>The records count.</value>
        public int RecordsCount { get; set; }
        /// <summary>Gets or sets the files count.</summary>
        /// <value>The files count.</value>
        public int FilesCount { get; set; }
    }
}