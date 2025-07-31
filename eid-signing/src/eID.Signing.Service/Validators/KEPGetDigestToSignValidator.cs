using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class KEPGetDigestToSignValidator : AbstractValidator<KEPGetDigestToSign>
{
    public KEPGetDigestToSignValidator()
    {
        RuleFor(r => r.DigestToSign)
            .NotEmpty();

        RuleFor(r => r.DocumentName)
            .NotEmpty();

        RuleFor(r => r.SigningCertificate)
            .NotEmpty();

        RuleFor(r => r.CertificateChain)
            .NotEmpty()
            .ForEach(r => r.NotEmpty());

        RuleFor(r => r.EncryptionAlgorithm)
            .NotEmpty();

        RuleFor(r => r.SigningDate)
            .NotEmpty();
    }
}
