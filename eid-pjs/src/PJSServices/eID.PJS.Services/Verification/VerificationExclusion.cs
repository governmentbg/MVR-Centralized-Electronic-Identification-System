using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

#nullable disable

namespace eID.PJS.Services.Verification;

/// <summary>
///   Defines the base class of the verification exclusions
/// </summary>
public abstract class VerificationExclusion : IValidatableRequest
{
    /// <summary>
    /// Gets or sets the exclusion identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    ///   <para>
    /// Gets or sets the date created.
    /// </para>
    /// </summary>
    /// <value>The date created.</value>
    public DateTime DateCreated { get; set; }
    /// <summary>Gets or sets the user who created the exclusion.</summary>
    /// <value>The user created the exclusion.</value>
    public string CreatedBy { get; set; }
    public string ExclusionType { get; }
    /// <summary>
    /// Brief explanation why is it being excluded from the process
    /// </summary>
    public string ReasonForExclusion { get; set; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    /// <returns></returns>
    public virtual IValidator GetValidator() => new ExclusionValidator<VerificationExclusion>();
}

/// <summary>
/// VerificationExclusion Validator
/// </summary>
/// <seealso cref="AbstractValidator&lt;VerificationExclusion&gt;" />
public class ExclusionValidator<T> : AbstractValidator<T> where T : VerificationExclusion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileORFolderExclusionValidator"/> class.
    /// </summary>
    public ExclusionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CreatedBy).NotEmpty();
    }
}


