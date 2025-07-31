using FluentValidation;

#nullable disable

namespace eID.PJS.Services.Verification;

/// <summary>
///   Defines exclusion based on a path. 
///   The path can be a fodler or file.
/// </summary>
public class FileORFolderExclusion : VerificationExclusion, IValidatableRequest
{
    /// <summary>
    /// Gets or sets the excluded path.
    /// </summary>
    /// <value>
    /// The excluded path.
    /// </value>
    public string ExcludedPath { get; set; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    /// <returns></returns>
    public override IValidator GetValidator() => new FileORFolderExclusionValidator();
}

/// <summary>
/// FileORFolderExclusion validator.
/// </summary>
/// <seealso cref="AbstractValidator&lt;FileORFolderExclusion&gt;" />
public class FileORFolderExclusionValidator : ExclusionValidator<FileORFolderExclusion>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileORFolderExclusionValidator"/> class.
    /// </summary>
    public FileORFolderExclusionValidator() : base()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CreatedBy).NotEmpty();
        RuleFor(r => r.ExcludedPath).NotEmpty();
        RuleFor(r => r.ReasonForExclusion).NotEmpty().MaximumLength(256);
    }
}
