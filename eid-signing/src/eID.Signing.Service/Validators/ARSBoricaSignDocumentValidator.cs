using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class ARSBoricaSignDocumentValidator : AbstractValidator<BoricaARSSignDocument>
{
    public ARSBoricaSignDocumentValidator()
    {
        RuleFor(r => r.Contents)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .ForEach(r => r.NotEmpty())
             .ForEach(r => r.SetValidator(new BoricaContentToSignValidator()));
    }
}
