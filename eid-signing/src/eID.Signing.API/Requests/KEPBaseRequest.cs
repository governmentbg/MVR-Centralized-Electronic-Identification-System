using FluentValidation;

namespace eID.Signing.API.Requests;

public class KEPBaseRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new KEPBaseRequestValidator();

    public string? SigningCertificate { get; set; }

    public IEnumerable<string>? CertificateChain { get; set; }

    /// <summary>
    /// eu.europa.esig.dss.enumerations.EncryptionAlgorithm
    /// DSA, ECDSA, EDDSA, HMAC, PLAIN_ECDSA, RSA, X25519, X448
    /// https://ec.europa.eu/digital-building-blocks/DSS/webapp-demo/apidocs/eu/europa/esig/dss/enumerations/EncryptionAlgorithm.html
    /// </summary>
    public string? EncryptionAlgorithm { get; set; }

    public DateTime SigningDate { get; set; }
}

public class KEPBaseRequestValidator : AbstractValidator<KEPBaseRequest>
{
    public KEPBaseRequestValidator()
    {
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
