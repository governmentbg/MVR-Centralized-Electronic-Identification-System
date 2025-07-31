using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing document with KEP
/// </summary>
public class KEPSignDigestRequest : KEPGetDigestRequest
{
    public override IValidator GetValidator() => new KEPSignDigestRequestValidator();

    public string? SignatureValue { get; set; }
}


public class KEPSignDigestRequestValidator : AbstractValidator<KEPSignDigestRequest>
{
    public KEPSignDigestRequestValidator()
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
