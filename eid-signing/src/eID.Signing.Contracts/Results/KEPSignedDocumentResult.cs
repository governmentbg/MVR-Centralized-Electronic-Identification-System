
namespace eID.Signing.Contracts.Results;
public class KEPSignedDocumentResult
{
    /// <summary>
    /// The document name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// eu.europa.esig.dss.model.MimeType
    /// https://ec.europa.eu/digital-building-blocks/DSS/webapp-demo/apidocs/eu/europa/esig/dss/enumerations/MimeTypeEnum.html
    /// </summary>
    public MimeType MimeType { get; set; }

    /// <summary>
    /// Binaries of the document or its digest value (for DigestDocument)
    /// </summary>
    public string Bytes { get; set; }

    /// <summary>
    /// The used DigestAlgorithm in case of a DigestDocument (allows to send only the digest of the document)
    /// eu.europa.esig.dss.enumerations.DigestAlgorithm
    /// SHA1, SHA224, SHA256, SHA3_224, SHA3_256, SHA3_384, SHA3_512, SHA384, SHA512, SHAKE128, SHAKE256, SHAKE256_512, WHIRLPOOL
    /// https://ec.europa.eu/digital-building-blocks/DSS/webapp-demo/apidocs/eu/europa/esig/dss/enumerations/DigestAlgorithm.html
    /// </summary>
    public string DigestAlgorithm { get; set; }
}
