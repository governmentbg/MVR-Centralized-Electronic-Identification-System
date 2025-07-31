using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface KEPBaseCommand : CorrelatedBy<Guid>
{
    public string SigningCertificate { get; set; }

    public IEnumerable<string> CertificateChain { get; set; }

    /// <summary>
    /// eu.europa.esig.dss.enumerations.EncryptionAlgorithm
    /// DSA, ECDSA, EDDSA, HMAC, PLAIN_ECDSA, RSA, X25519, X448
    /// https://ec.europa.eu/digital-building-blocks/DSS/webapp-demo/apidocs/eu/europa/esig/dss/enumerations/EncryptionAlgorithm.html
    /// </summary>
    public string EncryptionAlgorithm { get; set; }

    public DateTime SigningDate { get; set; }
}
