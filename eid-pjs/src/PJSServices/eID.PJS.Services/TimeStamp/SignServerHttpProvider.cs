using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Buffers.Text;
using System.Net.Http;

namespace eID.PJS.Services.TimeStamp
{
    /// <summary>Provider for the SignServer Http protocol</summary>
    public class SignServerHttpProvider : SignServerProviderBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SignServerHttpProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="serviceProvider"></param>
        public SignServerHttpProvider(SignServerProviderSettings settings, 
                                    IHttpClientFactory httpClientFactory, 
                                    IServiceProvider serviceProvider) : base(settings, httpClientFactory, serviceProvider)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignServerHttpProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="serviceProvider"></param>
        public SignServerHttpProvider(SignServerProviderSettings settings, IServiceProvider serviceProvider) : base(settings, serviceProvider)
        {

        }

        /// <summary>
        /// Requests the token from the SignServer sending the audit log file hash.
        /// </summary>
        /// <param name="auditLogFileHash">The audit log file hash.</param>
        /// <returns></returns>
        public override async Task<TimeStampTokenResult> RequestToken(string auditLogFileHash)
        {
            var result = new TimeStampTokenResult();

            try
            {

                byte[] data = Convert.FromBase64String(auditLogFileHash);

                var request = Rfc3161TimestampRequest.CreateFromData(data, HashAlgorithmName.SHA256, requestSignerCertificates: true);

                var resultData = new byte[data.Length];
                int bytesWritten = 0;

                if (request.TryEncode(resultData, out bytesWritten))
                {
                    var requestData = new Span<byte>(resultData, 0, bytesWritten).ToArray();

                    result.Request = Convert.ToBase64String(resultData);

                    _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/timestamp-query");

                    var msg = new HttpRequestMessage(HttpMethod.Post, _settings.RequestTokenUrl);
                    msg.Content = new ByteArrayContent(requestData);

                    var response = await _httpClient.SendAsync(msg);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsByteArrayAsync();
                        result.Response = Convert.ToBase64String(responseData);

                        Rfc3161TimestampToken timestampToken = request.ProcessResponse(responseData, out _);

                        X509Certificate2? timestampAuthorityCertificate;
                        if (timestampToken.VerifySignatureForData(data, out timestampAuthorityCertificate))
                        {
                            result.TimestampToken = timestampToken;
                        }
                        else
                        {
                            result.Error = new Exception($"Timestamp could not be verified for audit log hash '{auditLogFileHash}'");
                        }
                    }
                    else
                    {
                        result.Error = new Exception($"Failed to create the timestamp token for audit log hash '{auditLogFileHash}'. HttpStatus: {response.StatusCode}; Reason: {response.ReasonPhrase}");

                    }
                }
                else
                {
                    result.Error = new Exception($"Failed to encode the request for audit log hash '{auditLogFileHash}'");
                }
            }
            catch (Exception ex)
            {
                result.Error = new Exception($"Failed to create the timestamp token for audit log hash '{auditLogFileHash}'", ex);
            }

            return result;
        }
    }
}
