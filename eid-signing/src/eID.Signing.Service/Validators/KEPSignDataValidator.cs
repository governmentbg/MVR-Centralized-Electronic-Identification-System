using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class KEPSignDataValidator : AbstractValidator<KEPSignData>
{
    public KEPSignDataValidator()
    {
        RuleFor(r => r.DocumentToSign)
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
