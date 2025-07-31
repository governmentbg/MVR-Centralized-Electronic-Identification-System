using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing document with KEP
/// </summary>
public class KEPSignDataRequest : KEPGetDataRequest
{
    public override IValidator GetValidator() => new KEPSignDataRequestValidator();

    public string? SignatureValue { get; set; }
}


public class KEPSignDataRequestValidator : AbstractValidator<KEPSignDataRequest>
{
    public KEPSignDataRequestValidator()
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
