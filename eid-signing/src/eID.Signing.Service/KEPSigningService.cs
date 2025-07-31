using System.Text;
using eID.Signing.API.Requests;
using eID.Signing.Application.Options;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using eID.Signing.Service.Validators;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;

namespace eID.Signing.Service;

public class KEPSigningService : BaseService
{
    private readonly ILogger<KEPSigningService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ApplicationUrls _applicationUrls;

    public KEPSigningService(
        ILogger<KEPSigningService> logger,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IOptions<ApplicationUrls> applicationUrls)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient();
        _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
        _applicationUrls.Validate();
    }


    public async Task<ServiceResult<KEPDataToSignResult>> GetDataToSignAsync(KEPGetDataToSign message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new KEPGetDataToSignValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<KEPDataToSignResult>(validationResult.Errors);
        }

        // Action
        var documentUrl = $"{_applicationUrls.KEPUrl}/document/data";
        var httpSignDocBody = BuildGetDataHttpBody(message);

        _logger.LogInformation("Attempting to get data to sign by local KEP");

        try
        {
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed to get data to sign by local KEP. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(documentUrl),
                Content = new StringContent(JsonConvert.SerializeObject(httpSignDocBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Get data to sign by local KEP failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<KEPDataToSignResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Getting data to sign by local KEP returned empty response body");
                return BadGateway<KEPDataToSignResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<KEPDataToSignResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Getting data to sign by local KEP returned null response body");
                return BadGateway<KEPDataToSignResult>();
            }
            _logger.LogInformation("Getting data to sign by local KEP completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to get data to sign by local KEP. Request: POST {Url}, Body {Body}", documentUrl, httpSignDocBody);
            return UnhandledException<KEPDataToSignResult>();
        }
    }

    public async Task<ServiceResult<KEPSignedDocumentResult>> SignDataAsync(KEPSignData message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new KEPSignDataValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<KEPSignedDocumentResult>(validationResult.Errors);
        }

        // Action
        var documentUrl = $"{_applicationUrls.KEPUrl}/document/sign";
        var httpSignDocBody = BuildSignDataHttpBody(message);

        _logger.LogInformation("Attempting data sign by local KEP.");

        try
        {
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed data signing by local KEP. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(documentUrl),
                Content = new StringContent(JsonConvert.SerializeObject(httpSignDocBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Data signing by local KEP failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<KEPSignedDocumentResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Data signing by local KEP returned empty response body");
                return BadGateway<KEPSignedDocumentResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<KEPSignedDocumentResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Data signing by local KEP returned null response body");
                return BadGateway<KEPSignedDocumentResult>();
            }
            _logger.LogInformation("Data signing by local KEP completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to sign data by local KEP. Request: POST {Url}, Body {Body}", documentUrl, httpSignDocBody);
            return UnhandledException<KEPSignedDocumentResult>();
        }
    }

    public async Task<ServiceResult<KEPDataToSignResult>> GetDigestToSignAsync(KEPGetDigestToSign message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new KEPGetDigestToSignValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<KEPDataToSignResult>(validationResult.Errors);
        }

        // Action
        var digestUrl = $"{_applicationUrls.KEPUrl}/digest/data";
        var httpSignDigestBody = BuildGetDigestHttpBody(message);

        _logger.LogInformation("Attempting to get digest to sign by local KEP");

        try
        {
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed to get digest to sign by local KEP. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(digestUrl),
                Content = new StringContent(JsonConvert.SerializeObject(httpSignDigestBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Get digest to sign by local KEP failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<KEPDataToSignResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Getting digest to sign by local KEP returned empty response body");
                return BadGateway<KEPDataToSignResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<KEPDataToSignResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Getting digest to sign by local KEP returned null response body");
                return BadGateway<KEPDataToSignResult>();
            }
            _logger.LogInformation("Getting digest to sign by local KEP completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to get digest to sign by local KEP. Request: POST {Url}, Body {Body}", digestUrl, httpSignDigestBody);
            return UnhandledException<KEPDataToSignResult>();
        }
    }

    public async Task<ServiceResult<KEPSignedDocumentResult>> SignDigestAsync(KEPSignDigest message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new KEPSignDigestValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<KEPSignedDocumentResult>(validationResult.Errors);
        }

        // Action
        var digestUrl = $"{_applicationUrls.KEPUrl}/digest/sign";
        var httpSignDigestBody = BuildSignDigestHttpBody(message);

        _logger.LogInformation("Attempting digest sign by local KEP.");

        try
        {
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != System.Net.HttpStatusCode.BadRequest)
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed digest signing by local KEP. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(digestUrl),
                Content = new StringContent(JsonConvert.SerializeObject(httpSignDigestBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Digest signing by local KEP failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<KEPSignedDocumentResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Digest signing by local KEP returned empty response body");
                return BadGateway<KEPSignedDocumentResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<KEPSignedDocumentResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Digest signing by local KEP returned null response body");
                return BadGateway<KEPSignedDocumentResult>();
            }
            _logger.LogInformation("Digest signing by local KEP completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to sign digest by local KEP. Request: POST {Url}, Body {Body}", digestUrl, httpSignDigestBody);
            return UnhandledException<KEPSignedDocumentResult>();
        }
    }

    private static object BuildGetDataHttpBody(KEPGetDataToSign dataToSign)
    {
        var body = new
        {
            DocumentToSign = dataToSign.DocumentToSign,
            SigningCertificate = dataToSign.SigningCertificate,
            CertificateChain = dataToSign.CertificateChain,
            EncryptionAlgorithm = dataToSign.EncryptionAlgorithm,
            SigningDate = dataToSign.SigningDate
        };

        return body;
    }

    private static object BuildSignDataHttpBody(KEPSignData dataToSign)
    {
        var body = new
        {
            DocumentToSign = dataToSign.DocumentToSign,
            SigningCertificate = dataToSign.SigningCertificate,
            CertificateChain = dataToSign.CertificateChain,
            EncryptionAlgorithm = dataToSign.EncryptionAlgorithm,
            SignatureValue = dataToSign.SignatureValue,
            SigningDate = dataToSign.SigningDate
        };

        return body;
    }

    private static object BuildGetDigestHttpBody(KEPGetDigestToSign digestToSign)
    {
        var body = new
        {
            DigestToSign = digestToSign.DigestToSign,
            DocumentName = digestToSign.DocumentName,
            SigningCertificate = digestToSign.SigningCertificate,
            CertificateChain = digestToSign.CertificateChain,
            EncryptionAlgorithm = digestToSign.EncryptionAlgorithm,
            SigningDate = digestToSign.SigningDate
        };

        return body;
    }

    private static object BuildSignDigestHttpBody(KEPSignDigest digestToSign)
    {
        var body = new
        {
            DigestToSign = digestToSign.DigestToSign,
            DocumentName = digestToSign.DocumentName,
            SigningCertificate = digestToSign.SigningCertificate,
            CertificateChain = digestToSign.CertificateChain,
            EncryptionAlgorithm = digestToSign.EncryptionAlgorithm,
            SignatureValue = digestToSign.SignatureValue,
            SigningDate = digestToSign.SigningDate
        };

        return body;
    }
}


