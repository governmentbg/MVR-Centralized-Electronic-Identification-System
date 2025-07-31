using FluentValidation;

namespace eID.Signing.API.Public.Requests;

/// <summary>
/// Used for signing document with KEP
/// </summary>
public class KEPGetDataRequest : KEPBaseRequest
{
    public override IValidator GetValidator() => new KEPGetDataRequestValidator();

    public string? DocumentToSign { get; set; }
}


public class KEPGetDataRequestValidator : AbstractValidator<KEPGetDataRequest>
{
    public KEPGetDataRequestValidator()
    {
        RuleFor(r => r.DocumentToSign)
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
