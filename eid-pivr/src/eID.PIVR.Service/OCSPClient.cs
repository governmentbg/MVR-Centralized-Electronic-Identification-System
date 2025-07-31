using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;

namespace eID.PIVR.Service;

public class OcspClient
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    public OcspClient(ILogger logger, HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<OcspStatusType> GetOcspStatusAsync(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
    {
        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        var issuer = GetIssuerCertificate(certificate);

        if (issuer == null)
        {
            var message = "Issuer certificate is not found";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), message);

            throw new OcspClientException(message);
        }

        return await ValidateOCSPAsync(new X509Certificate(issuer.RawData), new X509Certificate(certificate.RawData));
    }

    private System.Security.Cryptography.X509Certificates.X509Certificate2? GetIssuerCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
    {
        // Self Signed Certificate
        if (cert.Subject == cert.Issuer)
        {
            return cert;
        }

        using var chain = new System.Security.Cryptography.X509Certificates.X509Chain();
        chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
        try
        {
            chain.Build(cert);
        }
        catch (Exception ex)
        {
            var message = $"An error occurred when trying to build the certificate chain: {ex.Message}";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), message);

            throw new OcspClientException(message, ex);
        }

        if (chain.ChainElements.Count <= 1)
        {
            var message = $"Chain elements are {chain.ChainElements.Count}. They must be more than 1";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), message);
            if (chain.ChainElements.Any())
            {
                var certificate = chain.ChainElements[0].Certificate;
                _logger.LogInformation("ChainElements[0].Certificate Base64: {Base64Certificate}", Convert.ToBase64String(certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pkcs12)));
            }
            throw new OcspClientException(message);
        }

        // Standard flow of chain elements is: UserCert[0], IssuerCert[1], RootCert[2]
        return chain.ChainElements[1].Certificate;
    }

    private async Task<OcspStatusType> ValidateOCSPAsync(X509Certificate issuerCert, X509Certificate cert)
    {
        var urls = GetAuthorityInformationAccessOcspUrl(cert);
        if (urls.Count == 0)
        {
            var urlMessage = $"OCSP URL is not found";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), urlMessage);

            throw new OcspClientException(urlMessage);
        }

        var url = urls[0];

        _logger.LogDebug("{Client}: OCSP ULR is {URL}", nameof(OcspClient), url);

        // Prepare request
        var reqGen = new OcspReqGenerator();
        var certId = new CertificateID(CertificateID.HashSha1, issuerCert, cert.SerialNumber);
        reqGen.AddRequest(certId);

        var nonce = new X509Extension(false, new DerOctetString(cert.SerialNumber.ToByteArray()));
        reqGen.SetRequestExtensions(new X509Extensions(new Dictionary<DerObjectIdentifier, X509Extension> {
            { OcspObjectIdentifiers.PkixOcspNonce, nonce } }));

        var req = reqGen.Generate();
        var reqData = req.GetEncoded();

        var byteContent = new ByteArrayContent(reqData);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/ocsp-request");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = byteContent,
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            var sendMessage = $"An error occurred when requesting {url}: {ex.Message}";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), sendMessage);

            throw new OcspClientException(sendMessage, ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var respMessage = $"Http response was not successful. URL: {url}. StatusCode: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), respMessage);

            throw new OcspClientException(respMessage);
        }

        _logger.LogDebug("{Client}: {Url} request was successful", nameof(OcspClient), url);

        var ocspResp = new OcspResp(response.Content.ReadAsStream());
        if (ocspResp.Status != OcspRespStatus.Successful)
        {
            var ocspMessage = $"Ocsp response is not successful. Status: {ocspResp.Status}. InternalError[2], TryLater[3], SigRequired[5], Unauthorized[6]";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), ocspMessage);

            throw new OcspClientException(ocspMessage);
        }

        var basicOcspResp = (BasicOcspResp)ocspResp.GetResponseObject();
        if (basicOcspResp == null)
        {
            var ocspRespObjMessage = $"Ocsp response type is null. Type: {ocspResp.GetResponseObject()}. ";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), ocspRespObjMessage);

            throw new OcspClientException(ocspRespObjMessage);
        }

        if (basicOcspResp.Responses.Length != 1)
        {
            var ocspRespMessage = $"Ocsp responses must be 1. They are: {basicOcspResp.Responses.Length}";
            _logger.LogError("{Client}: {Message}", nameof(OcspClient), ocspRespMessage);

            throw new OcspClientException(ocspRespMessage);
        }

        var certificateStatus = basicOcspResp.Responses[0].GetCertStatus();
        _logger.LogDebug("{Client}: Certificate status is {Status}", nameof(OcspClient), certificateStatus);

        if (certificateStatus == null || certificateStatus == CertificateStatus.Good)
        {
            return OcspStatusType.Good;
        }

        if (certificateStatus is RevokedStatus)
        {
            return OcspStatusType.Revoked;
        }

        if (certificateStatus is UnknownStatus)
        {
            return OcspStatusType.Unknown;
        }

        var message = $"Certificate status {certificateStatus} is unhandled";
        _logger.LogError("{Client}: {Message}", nameof(OcspClient), message);

        throw new OcspClientException(message);
    }

    private static List<string> GetAuthorityInformationAccessOcspUrl(X509Certificate cert)
    {
        var ocspUrls = new List<string>();

        var asnObject = GetExtensionValue(cert, X509Extensions.AuthorityInfoAccess.Id);

        if (asnObject == null)
        {
            return ocspUrls;
        }

        var asnSequence = (Asn1Sequence)asnObject;

        foreach (var element in asnSequence.Cast<Asn1Sequence>())
        {
            var oid = (DerObjectIdentifier)element[0];

            if (oid != null && oid.Id.Equals("1.3.6.1.5.5.7.48.1"))
            {
                var taggedObject = (Asn1TaggedObject)element[1];
                var generalName = GeneralName.GetInstance(taggedObject);
                var instance = DerIA5String.GetInstance(generalName.Name);
                ocspUrls.Add(instance.GetString());
            }
        }

        return ocspUrls;
    }

    private static Asn1Object? GetExtensionValue(X509Certificate cert, string oid)
    {
        byte[]? bytes = cert.GetExtensionValue(new DerObjectIdentifier(oid))?.GetOctets();

        if (bytes == null)
        {
            return null;
        }

        return Asn1Object.FromByteArray(bytes);
    }
}

public enum OcspStatusType
{
    None = 0,
    Unknown = 1,
    Revoked = 2,
    Good = 3
}

[Serializable]
public class OcspClientException : Exception
{
    public OcspClientException(string? message) : base(message)
    {
    }

    public OcspClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected OcspClientException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
