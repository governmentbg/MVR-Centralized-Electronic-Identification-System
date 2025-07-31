using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable  disable

namespace eID.PJS.LocalLogsSearch.Service.Requests
{
    /// <summary>
    /// Request to retreive the content of the file
    /// </summary>
    /// <seealso cref="eID.PJS.LocalLogsSearch.Service.IValidatableRequest" />
    public class FileContentRequest : IValidatableRequest
    {
        /// <summary>
        /// The full path to the file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the assigned validator to validate the request
        /// </summary>
        /// <returns>Instance of FileContentRequestValidator</returns>
        public virtual IValidator GetValidator() => new FileContentRequestValidator();
    }


    /// <summary>
    /// Validator for the FileContentRequest
    /// </summary>
    public class FileContentRequestValidator : AbstractValidator<FileContentRequest>
    {
        /// <summary>
        /// SearchLogsRequestValidator empty constructor
        /// </summary>
        public FileContentRequestValidator()
        {
            RuleFor( f=> f.FilePath).NotEmpty();
        }
    }
}
