using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.PDEAU.Service;

public class TimestampService : BaseService, ITimestampService
{
    private readonly ILogger<TimestampService> _logger;
    private readonly HttpClient _timestampServerHttpClient;
    private readonly TimestampServerOptions _timestampServerOptions;

    public TimestampService(
        ILogger<TimestampService> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<TimestampServerOptions> timestampServerOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _timestampServerHttpClient = httpClientFactory.CreateClient(TimestampServerOptions.HTTP_CLIENT_NAME);

        _timestampServerOptions = timestampServerOptions?.Value ?? new TimestampServerOptions();
        _timestampServerOptions.Validate();
    }

    public async Task<ServiceResult<string>> SignDataAsync(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException($"'{nameof(data)}' cannot be null or empty.", nameof(data));
        }

        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(dataBytes);
        var timestampRequest = Rfc3161TimestampRequest.CreateFromData(hash, HashAlgorithmName.SHA256, requestSignerCertificates: true);
        var encodedRequest = timestampRequest.Encode();
        var base64Request = Convert.ToBase64String(encodedRequest);

        var request = new HttpRequestMessage(HttpMethod.Post, _timestampServerOptions.RequestTokenUrl)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                data = base64Request,
                encoding = "BASE64"
            }), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        HttpResponseMessage response;
        try
        {
            response = await _timestampServerHttpClient.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server failed.");
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "HTTP request to timestamping server timed out.");
            return new ServiceResult<string> { StatusCode = HttpStatusCode.RequestTimeout };
        }
        catch (Exception ex)
        {
            var logMessage = "An unexpected error occurred during the timestamping server call.";
            _logger.LogError(ex, logMessage);
            return UnhandledException<string>(logMessage);
        }

        var responseRawData = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Timestamping call failed. StatusCode: {StatusCode}; Response raw data: {ResponseRawData}", response.StatusCode, responseRawData);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }

        var responseObj = JsonConvert.DeserializeObject<TimestampServerTokenResponse>(responseRawData);
        if (responseObj is null)
        {
            _logger.LogWarning("Deserialization of timestamp token response failed. Response raw data: {ResponseRawData}", responseRawData);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }

        if (string.IsNullOrEmpty(responseObj.Data))
        {
            _logger.LogWarning("Timestamp signature for data is empty. Response raw data: {ResponseRawData}", responseRawData);
            return new ServiceResult<string> { StatusCode = HttpStatusCode.BadGateway };
        }

        return Ok(responseObj.Data);
    }

    public bool VerifyTimestampSignature(string data, string signature)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException($"'{nameof(data)}' cannot be null or empty.", nameof(data));
        }

        if (string.IsNullOrEmpty(signature))
        {
            throw new ArgumentException($"'{nameof(signature)}' cannot be null or empty.", nameof(signature));
        }

        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(dataBytes);
        var timestampRequest = Rfc3161TimestampRequest.CreateFromData(hash, HashAlgorithmName.SHA256, requestSignerCertificates: true);

        var signatureBytes = Convert.FromBase64String(signature);
        var timestampToken = timestampRequest.ProcessResponse(signatureBytes, out _);

        var result = timestampToken.VerifySignatureForData(hash, out _);
        _logger.LogDebug("The signature is valid: {result}", result);

        return result;
    }

    private class TimestampServerTokenResponse
    {
        public string Data { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public string ArchiveId { get; set; } = string.Empty;
        public string SignerCertificate { get; set; } = string.Empty;
    }
}

public interface ITimestampService
{
    Task<ServiceResult<string>> SignDataAsync(string data);
    bool VerifyTimestampSignature(string data, string signature);
}
