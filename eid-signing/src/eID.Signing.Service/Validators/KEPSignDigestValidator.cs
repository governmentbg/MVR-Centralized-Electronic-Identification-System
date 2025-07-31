using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class KEPSignDigestValidator : AbstractValidator<KEPSignDigest>
{
    public KEPSignDigestValidator()
    {
        RuleFor(r => r.DigestToSign)
            .NotEmpty();

        RuleFor(r => r.DocumentName)
            .NotEmpty();

        RuleFor(r => r.SignatureValue)
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
