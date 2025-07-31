using eID.Signing.Contracts.Commands;
using eID.Signing.Service.Validators;
using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing document(s)
/// </summary>
public class EvrotrustSignDocumentRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new EvrotrustSignDocumentRequestValidator();

    /// <summary>
    /// Signing request will expire in this moment
    /// </summary>
    public DateTime DateExpire { get; set; }
    /// <summary>
    /// List with documents to be signed
    /// </summary>
    public IEnumerable<EvrotrustDocumentRequest>? Documents { get; set; }
    /// <summary>
    /// Uid of the user performing the signing operation. Override when available in the token
    /// </summary>
    public string Uid { get; set; }
}

/// <summary>
/// Represents document to be signed
/// </summary>
public class EvrotrustDocumentRequest : EvrotrustDocumentToSign
{
    /// <summary>
    /// Hash of the document content
    /// </summary>
    public string? Content { get; set; }
    /// <summary>
    /// File name with extension
    /// </summary>
    public string? FileName { get; set; }
    /// <summary>
    /// Content type of the file
    /// </summary>
    public string? ContentType { get; set; }
}

public class EvrotrustSignDocumentRequestValidator : AbstractValidator<EvrotrustSignDocumentRequest>
{
    public EvrotrustSignDocumentRequestValidator()
    {
        RuleFor(r => r.DateExpire)
            .NotEmpty()
            .GreaterThan(r => DateTime.UtcNow);

        RuleFor(r => r.Documents)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r => r.NotEmpty())
            .ForEach(r => r.SetValidator(new EvrotrustDocumentRequestValidator()));

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
    }
}

public class EvrotrustDocumentRequestValidator : AbstractValidator<EvrotrustDocumentRequest>
{
    public EvrotrustDocumentRequestValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty();

        RuleFor(r => r.FileName)
            .NotEmpty();

        RuleFor(r => r.ContentType)
            .NotEmpty();
    }
}
