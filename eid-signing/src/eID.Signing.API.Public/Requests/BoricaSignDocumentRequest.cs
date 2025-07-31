using eID.Signing.Contracts.Commands;
using eID.Signing.Service.Validators;
using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing document(s)
/// </summary>
public class BoricaSignDocumentRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new BoricaSignDocumentRequestValidator();

    
    /// <summary>
    /// List with documents to be signed
    /// </summary>
    
    public IEnumerable<BoricaContentRequest>? Contents { get; set; }
    /// <summary>
    /// Uid of the user performing the signing operation. Override when available in the token
    /// </summary>
    public string Uid { get; set; }
}

/// <summary>
/// Represents document to be signed
/// </summary>
public class BoricaContentRequest : BoricaContentToSign
{
}

public class BoricaSignaturePosition
{
    public int ImageHeight { get; set; }

    public int ImageWidth { get; set; }

    public int ImageXAxis { get; set; }

    public int ImageYAxis { get; set; }

    public int PageNumber { get; set; }
}

public class BoricaSignDocumentRequestValidator : AbstractValidator<BoricaSignDocumentRequest>
{
    public BoricaSignDocumentRequestValidator()
    {
        RuleFor(r => r.Contents)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .ForEach(r => r.NotEmpty())
             .ForEach(r => r.SetValidator(new BoricaContentRequestValidator()));

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
    }
}

public class BoricaContentRequestValidator : AbstractValidator<BoricaContentRequest>
{
    public BoricaContentRequestValidator()
    {
        RuleFor(r => r.ConfirmText)
            .NotEmpty();

        RuleFor(r => r.ContentFormat)
            .NotEmpty();

        RuleFor(r => r.MediaType)
            .NotEmpty();

        RuleFor(r => r.Data)
            .NotEmpty();

        RuleFor(r => r.FileName)
            .NotEmpty();

        RuleFor(r => r.PadesVisualSignature)
            .NotEmpty();
    }
}
