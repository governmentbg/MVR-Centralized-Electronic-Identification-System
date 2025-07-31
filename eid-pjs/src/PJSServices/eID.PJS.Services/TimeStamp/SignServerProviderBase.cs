using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using eID.PJS.Services.Entities;
using System.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace eID.PJS.Services.TimeStamp
{
    public abstract class SignServerProviderBase:  ITimeStampProvider
    {
        /// <summary>
        /// SignServerProviderSettings settings local instance
        /// </summary>
        protected SignServerProviderSettings _settings;
        protected HttpClient _httpClient;
        protected readonly IServiceProvider _serviceProvider;

        /// <summary>Initializes a new instance of the <see cref="SignServerProviderBase" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="serviceProvider"></param>
        /// <exception cref="System.ArgumentNullException">settings
        /// or
        /// BaseUrl</exception>
        public SignServerProviderBase(SignServerProviderSettings settings, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _settings.ValidateAndThrow();

            _httpClient = httpClientFactory.CreateClient(HttpClientNames.SignServerHttpClient);
        }

        /// <summary>Initializes a new instance of the <see cref="SignServerProviderBase" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="serviceProvider"></param>
        /// <exception cref="System.ArgumentNullException">settings
        /// or
        /// BaseUrl</exception>
        public SignServerProviderBase(SignServerProviderSettings settings, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _settings.ValidateAndThrow();

             _httpClient = new HttpClient();
        }

        /// <summary>
        /// Requests the token.
        /// </summary>
        /// <param name="auditLogFileHash">The audit log file hash.</param>
        /// <returns></returns>
        public abstract Task<TimeStampTokenResult> RequestToken(string auditLogFileHash);

        /// <summary>
        /// Verifies the token.
        /// </summary>
        /// <param name="auditLogFileHash">The audit log file hash.</param>
        /// <param name="encodedTokenResponse">The encoded token response.</param>
        /// <returns></returns>
        public virtual TimeStampVerificationResult VerifyToken(string auditLogFileHash, string encodedTokenResponse, X509Certificate2 signServerCertChain)
        {
            var result = new TimeStampVerificationResult();

            try
            {
                var data = Convert.FromBase64String(auditLogFileHash);
                var responseData = Convert.FromBase64String(encodedTokenResponse);

                var request = Rfc3161TimestampRequest.CreateFromData(data, HashAlgorithmName.SHA256, requestSignerCertificates: true);

                var resultData = new byte[data.Length];
                int bytesWritten = 0;

                if (request.TryEncode(resultData, out bytesWritten))
                {
                    Rfc3161TimestampToken timestampToken = request.ProcessResponse(responseData, out _);

                    result.TokenInfo = timestampToken.TokenInfo;

                    X509Certificate2? timestampAuthorityCertificate;

                    X509Certificate2Collection? certs = new X509Certificate2Collection(signServerCertChain);

                    if (timestampToken.VerifySignatureForData(data, out timestampAuthorityCertificate, certs))
                    {
                        result.IsValid = true;
                    }
                    else
                    {
                        result.Error = new Exception($"Timestamp could not be verified for audit log hash '{auditLogFileHash}' using timestamp token '{encodedTokenResponse}'");
                    }

                }
                else
                {
                    result.Error = new Exception($"Failed to encode the request for audit log hash '{auditLogFileHash}' using timestamp token '{encodedTokenResponse}'");
                }
            }
            catch (Exception ex)
            {
                result.Error = new Exception($"Error VerifyToken for audit log hash '{auditLogFileHash}' using timestamp token '{encodedTokenResponse}'", ex);
            }

            return result;
        }
    }

   
}
