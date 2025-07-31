using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing digest data with KEP
/// </summary>
public class KEPGetDigestRequest : KEPBaseRequest
{
    public override IValidator GetValidator() => new KEPGetDigestRequestValidator();

    public string? DigestToSign { get; set; }

    public string? DocumentName { get; set; }
}


public class KEPGetDigestRequestValidator : AbstractValidator<KEPGetDigestRequest>
{
    public KEPGetDigestRequestValidator()
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
