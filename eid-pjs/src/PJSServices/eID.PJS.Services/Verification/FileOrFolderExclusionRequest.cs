
using FluentValidation;

namespace eID.PJS.Services.Verification
{
    public class FileOrFolderExclusionRequest : IValidatableRequest
    {
        /// <summary>
        /// Gets or sets the excluded path.
        /// </summary>
        /// <value>
        /// The excluded path.
        /// </value>
        public string ExcludedPath { get; set; }
        /// <summary>
        /// Brief explanation why is it being excluded from the process
        /// </summary>
        public string ReasonForExclusion { get; set; }

        public virtual IValidator GetValidator() => new FileOrFolderExclusionRequestValidator();

    }

    public class FileOrFolderExclusionRequestValidator : AbstractValidator<FileOrFolderExclusionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileORFolderExclusionValidator"/> class.
        /// </summary>
        public FileOrFolderExclusionRequestValidator() : base()
        {
            RuleFor(r => r.ExcludedPath).NotEmpty();
            RuleFor(r => r.ReasonForExclusion).NotEmpty().MaximumLength(256);
        }
    }
}
