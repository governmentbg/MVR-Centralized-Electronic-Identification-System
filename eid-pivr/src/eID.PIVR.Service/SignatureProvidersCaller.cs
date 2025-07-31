using System.Net;
using System.Text;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Extensions;
using eID.PIVR.Service.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eID.PIVR.Service;

public interface ISignatureProvidersCaller
{
    Task<ServiceResult> EvrotrustUserCertificateCheckAsync(string uid, string certSn);
}

public class SignatureProvidersCaller : ISignatureProvidersCaller
{
    private readonly ILogger<SignatureProvidersCaller> _logger;
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ExternalRegistersCacheOptions _cacheOptions;

    public SignatureProvidersCaller(
        ILogger<SignatureProvidersCaller> logger,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IOptions<ExternalRegistersCacheOptions> cacheOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("Integrations");
        _cacheOptions = (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value;
    }

    public async Task<ServiceResult> EvrotrustUserCertificateCheckAsync(string uid, string certSn)
    {
        var userCertificateVerificationCacheKey = $"eID:PIVR:SignatureProvidersCaller:VerifyCertificate:Evrotrust:{CalculatedUidSha(uid)}:{certSn}";
        var userCertificateVerificationResponse = await _cache.GetOrCreateAsync(userCertificateVerificationCacheKey, async () =>
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_httpClient.BaseAddress}evrotrust/user/certificate/check"),
                Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        User = new { IdentificationNumber = uid },
                        Certificate = new { SerialNumber = certSn }
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            });
            var responseStr = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting user certificate check. ({StatusCode}) {Response}", response.StatusCode.ToString(), responseStr);
                return new UserCertificateCheckResponse();
            }

            var userCertificateCheckResponse = JsonConvert.DeserializeObject<UserCertificateCheckResponse>(responseStr) ?? new UserCertificateCheckResponse();
            await _cache.SetAsync(userCertificateVerificationCacheKey, userCertificateCheckResponse, _cacheOptions.ExpireAfterInHours);
            return userCertificateCheckResponse;
        });

        if (userCertificateVerificationResponse is null)
        {
            _logger.LogInformation("Evrotrust certificate validation. No response. {CertificateSerialNumber}", certSn);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = "No response"
            };
        }
        if (!userCertificateVerificationResponse.Result)
        {
            _logger.LogInformation("Evrotrust successful certificate validation. Invalid certificate. {CertificateSerialNumber}", certSn);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Invalid certificate"
            };
        }
        _logger.LogInformation("Evrotrust certificate validation completed. {CertificateSerialNumber}", certSn);
        return new ServiceResult { StatusCode = HttpStatusCode.OK };
    }

    private static string CalculatedUidSha(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException($"'{nameof(uid)}' cannot be null or whitespace.", nameof(uid));
        }

        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(uid));
            return BitConverter.ToString(hashBytes).Replace("-", String.Empty);
        }
    }
}
