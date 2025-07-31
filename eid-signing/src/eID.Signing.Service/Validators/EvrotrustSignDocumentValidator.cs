using eID.Signing.Contracts.Commands;
using eID.Signing.Service.Validators;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class EvrotrustSignDocumentValidator : AbstractValidator<EvrotrustSignDocument>
{
    public EvrotrustSignDocumentValidator()
    {
        RuleFor(r => r.DateExpire)
            .NotEmpty()
            .GreaterThan(r => DateTime.UtcNow);

        RuleFor(r => r.Documents)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .ForEach(r => r.NotEmpty())
             .ForEach(r => r.SetValidator(new EvrotrustDocumentToSignValidator()));

        RuleFor(r => r.UserIdentifiers)
             .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(r =>
                        r
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .Must(uid => ValidatorHelpers.UidFormatIsValid(uid))
                            .WithMessage("{PropertyName} is invalid.")
                        .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                            .WithMessage("{PropertyName} is below lawful age.")
            );
    }
}

public class EvrotrustDocumentToSignValidator : AbstractValidator<EvrotrustDocumentToSign>
{
    public EvrotrustDocumentToSignValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty();

        RuleFor(r => r.FileName)
            .NotEmpty();

        RuleFor(r => r.ContentType)
            .NotEmpty();
    }
}
