using eID.Signing.Contracts.Commands;
using eID.Signing.Service.Validators;
using FluentValidation;

namespace eID.Signing.API.Requests;
public class BoricaSignDocumentValidator : AbstractValidator<BoricaSignDocument>
{
    public BoricaSignDocumentValidator()
    {
        RuleFor(r => r.Contents)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .ForEach(r => r.NotEmpty())
             .ForEach(r => r.SetValidator(new BoricaContentToSignValidator()));

        RuleFor(r => r.Uid)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                .WithMessage("{PropertyName} is invalid.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} is below lawful age.");
    }
}

public class BoricaContentToSignValidator : AbstractValidator<BoricaContentToSign>
{
    public BoricaContentToSignValidator()
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
