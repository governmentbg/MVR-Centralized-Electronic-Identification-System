using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Enums;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Validators;
using Microsoft.Extensions.Logging;

namespace eID.PIVR.Service;

public class VerificationService : BaseService
{
    private readonly ILogger<VerificationService> _logger;
    private readonly ISignatureProvidersCaller _signatureProvidersCaller;
    private readonly OcspClient _ocspClient;

    public VerificationService(
        ILogger<VerificationService> logger,
        ISignatureProvidersCaller signatureProvidersCaller,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signatureProvidersCaller = signatureProvidersCaller ?? throw new ArgumentNullException(nameof(signatureProvidersCaller));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        var httpClient = httpClientFactory.CreateClient("Ocsp");
        _ocspClient = new OcspClient(_logger, httpClient);
    }

    /// <summary>
    /// Verifies a detached signature signs the original file content
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResult> VerifySignatureAsync(VerifySignature message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var validator = new VerifySignatureValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(VerifySignature), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var originalFileBytes = Encoding.UTF8.GetBytes(message.OriginalFile);
        var signatureBytes = default(byte[]);
        try
        {
            signatureBytes = Convert.FromBase64String(message.DetachedSignature);
        }
        catch (FormatException ex)
        {
            _logger.LogInformation(ex, "Signature invalid format");
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new (
                        $"Invalid signature format",
                        ex.Message
                    )
                }
            };
        }

        var contentInfo = new ContentInfo(originalFileBytes);
        var signedCms = new SignedCms(contentInfo, true);

        try
        {
            signedCms.Decode(signatureBytes);
            signedCms.CheckSignature(true);
            _logger.LogInformation("Successfully checked signature.");
        }
        catch (CryptographicException ex)
        {
            _logger.LogInformation(ex, "Failed verifying signature");
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = "Failed validating signature"
            };
        }

        // Detached signature it makes sense to be only one signer info.
        var signerInfo = signedCms.SignerInfos[0];

        if (signerInfo.Certificate == null)
        {
            _logger.LogInformation("Failed verifying signature. Signer certificate not found.");
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = "Signer certificate is not found."
            };
        }

        if (signerInfo.Certificate.Archived)
        {
            _logger.LogInformation("Certificate archived.");
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = new List<KeyValuePair<string, string>>
                        {
                            new (
                                $"Archived",
                                "Certificate archived."
                            )
                        }
            };
        }

        var expiredCertificate = DateTime.UtcNow > signerInfo.Certificate.NotAfter.ToUniversalTime();
        if (expiredCertificate)
        {
            _logger.LogInformation("Certificate expired. {ExpiryDate}", signerInfo.Certificate.NotAfter.ToUniversalTime());
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Errors = new List<KeyValuePair<string, string>>
                        {
                            new (
                                $"Expired",
                                $"Certificate expired. {signerInfo.Certificate.NotAfter.ToUniversalTime()}"
                            )
                        }
            };
        }

        OcspStatusType ocspResult;
        try
        {
            ocspResult = await _ocspClient.GetOcspStatusAsync(signerInfo.Certificate);
        }
        catch (OcspClientException ex)
        {
            _logger.LogInformation(ex, "Failed certificate status check");
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = ex.Message
            };
        }

        if (ocspResult != OcspStatusType.Good)
        {
            _logger.LogInformation("Bad OCSP certificate status is {CertificateStatus}", ocspResult);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadGateway,
                Errors = new List<KeyValuePair<string, string>>
                        {
                            new (
                                "Bad OCSP certificate status",
                                ocspResult.ToString()
                            )
                        }
            };
        }

        string subjectUidAffix = "PNOBG";
        if (message.UidType == IdentifierType.LNCh)
        {
            subjectUidAffix = "PI:BG";
        }

        if (!signerInfo.Certificate.Subject.Contains($"{subjectUidAffix}-{message.Uid}"))
        {
            _logger.LogInformation("Missing {SubjectUidAffix}-Uid in certificate subject.", subjectUidAffix);
            switch (message.SignatureProvider)
            {
                case SignatureProvider.KEP:
                case SignatureProvider.Borica:
                    _logger.LogInformation("Failed verifying signature.");
                    return new ServiceResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<KeyValuePair<string, string>>
                        {
                            new (
                                $"{nameof(signerInfo.Certificate)}.{nameof(signerInfo.Certificate.Subject)}",
                                "Certificate subject is missing uid information."
                            )
                        }
                    };
                case SignatureProvider.Evrotrust:
                    return await _signatureProvidersCaller.EvrotrustUserCertificateCheckAsync(message.Uid, signerInfo.Certificate.GetSerialNumberString());
                default:
                    throw new NotSupportedException("Signature provider not supported");
            }
        }

        _logger.LogInformation("Successfully verified certificate uid.");
        return Ok();
    }
}
