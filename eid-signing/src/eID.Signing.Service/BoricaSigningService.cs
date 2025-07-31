using System.Text;
using eID.Signing.API.Requests;
using eID.Signing.Application.Options;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Enums;
using eID.Signing.Contracts.Results;
using eID.Signing.Service.Database;
using eID.Signing.Service.Options;
using eID.Signing.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;

namespace eID.Signing.Service;

public class BoricaSigningService : BaseService
{
    private readonly ILogger<BoricaSigningService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ApplicationUrls _applicationUrls;
    private readonly AutomaticRemoteSigningOptions _automaticRemoteSigningOptions;

    public BoricaSigningService(
        ILogger<BoricaSigningService> logger,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IOptions<ApplicationUrls> applicationUrls,
        IOptions<AutomaticRemoteSigningOptions> automaticRemoteSigningOptions,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient(ApplicationConstants.HttpClientWithMetricsName);
        _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
        _applicationUrls.Validate();
        _automaticRemoteSigningOptions = (automaticRemoteSigningOptions ?? throw new ArgumentNullException(nameof(automaticRemoteSigningOptions))).Value;
        _automaticRemoteSigningOptions.Validate();
    }

    public async Task<ServiceResult<BoricaFileStatusResult>> GetFileStatusAsync(BoricaGetFileStatusByTransactionId message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new BoricaGetFileStatusByTransactyionIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<BoricaFileStatusResult>(validationResult.Errors);
        }

        // Action
        var getFileStatusUrl = $"{_applicationUrls.BoricaHostUrl}/sign/{message.TransactionId}";

        _logger.LogInformation("Attempting to get status for document signed by Borica online service.");

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
                                    "Failed getting status for document signed by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(getFileStatusUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting status for document signed by Borica: {Response}", responseStr);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaFileStatusResult>();
                }
            }

            response.EnsureSuccessStatusCode();

            var statusResult = JsonConvert.DeserializeObject<BoricaFileStatusResult>(responseStr) ?? new BoricaFileStatusResult();

            _logger.LogInformation("Getting status for document signed by Borica completed successfully");

            return Ok(statusResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to get status for document signed by Borica. Request: GET {Url}", getFileStatusUrl);
            return UnhandledException<BoricaFileStatusResult>();
        }
    }

    public async Task<ServiceResult<BoricaFileContent>> DownloadFileAsync(BoricaDownloadFileByTransactionId message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new BoricaDownloadFileByTransactyionIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<BoricaFileContent>(validationResult.Errors);
        }

        // Action
        var downloadFileUrl = $"{_applicationUrls.BoricaHostUrl}/sign/content/{message.TransactionId}";

        _logger.LogInformation("Attempting to download document signed by Borica online service.");

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
                                    "Failed downloading document signed by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(downloadFileUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed downloading document signed by Borica: {Response}", responseStr);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaFileContent>();
                }
            }
            response.EnsureSuccessStatusCode();

            var documentResult = JsonConvert.DeserializeObject<BoricaFileContent>(responseStr, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }) ?? new BoricaFileContent();

            _logger.LogInformation("Downloading document signed by Borica completed successfully");

            return Ok(documentResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when downloading document signed by Borica. Request: GET {Url}", downloadFileUrl);
            return UnhandledException<BoricaFileContent>();
        }
    }

    public async Task<ServiceResult<BoricaDocumentSigningResult>> SignDocumentAsync(BoricaSignDocument message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new BoricaSignDocumentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<BoricaDocumentSigningResult>(validationResult.Errors);
        }

        // Action
        var signingUrl = $"{_applicationUrls.BoricaHostUrl}/sign?rpToClientAuthorization=personalId:{message.Uid}";
        var httpSignDocBody = BuildSignDocumentHttpBody(message);

        _logger.LogInformation("Attempting document sign by Borica online service.");

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
                                    "Failed document signing by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(signingUrl),
                Content = new StringContent(JsonConvert.SerializeObject(httpSignDocBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Document signing by Borica failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaDocumentSigningResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Document signing by Borica returned empty response body");
                return BadGateway<BoricaDocumentSigningResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<BoricaDocumentSigningResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Document signing by Borica returned null response body");
                return BadGateway<BoricaDocumentSigningResult>();
            }
            _logger.LogInformation("Document signing by Borica completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to sign document by Borica. Request: POST {Url}, Body {Body}", signingUrl, httpSignDocBody);
            return UnhandledException<BoricaDocumentSigningResult>();
        }
    }

    public async Task<ServiceResult<BoricaCertificateCheckResult>> CheckUserAsync(BoricaCheckUserByUid message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new BoricaCheckUserByUidValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<BoricaCertificateCheckResult>(validationResult.Errors);
        }
        var idType = string.Empty;
        if (ValidatorHelpers.EgnFormatIsValid(message.Uid))
        {
            idType = "EGN";
        }
        else if (ValidatorHelpers.LnchFormatIsValid(message.Uid))
        {
            idType = "LNC";
        }
        if (string.IsNullOrEmpty(idType))
        {
            _logger.LogWarning("Validator didn't catch invalid Uid when checking user for Borica");
            return UnhandledException<BoricaCertificateCheckResult>();
        }

        // Action
        var checkIdentityUrl = $"{_applicationUrls.BoricaHostUrl}/cert/identity/{idType}/{message.Uid}";

        _logger.LogInformation("Attempting to check certificate identity for user by Uid in Borica online service...");

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
                                    "Failed check certificate identity for user by Uid in Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(checkIdentityUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed check certificate identity for user by Uid in Borica: {Response}", responseStr);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaCertificateCheckResult>();
                }
            }
            response.EnsureSuccessStatusCode();

            var certificateResult = JsonConvert.DeserializeObject<BoricaCertificateCheckResult>(responseStr) ?? new BoricaCertificateCheckResult();

            _logger.LogInformation("Checking certificate identity for user by Uid in Borica completed successfully");

            return Ok(certificateResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to check certificate identity for user by Uid in Borica. Request: GET {Url}", checkIdentityUrl);
            return UnhandledException<BoricaCertificateCheckResult>();
        }
    }

    public async Task<ServiceResult<BoricaSendConsentResult>> SendConsentAsync()
    {
        var url = $"{_applicationUrls.BoricaHostUrl}/consent?rpToClientAuthorization=certId:{_automaticRemoteSigningOptions.AuthorizedPersonCertId}";
        _logger.LogInformation("Attempting to send consent by Borica online service.");

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
                                    "Failed sending consent by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    ExpirationTime = _automaticRemoteSigningOptions.ExpirationTimeInDays,
                    _automaticRemoteSigningOptions.Organisation,
                    _automaticRemoteSigningOptions.Role,
                    _automaticRemoteSigningOptions.Subject
                }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Sending consent by Borica failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaSendConsentResult>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Sending consent by Borica returned empty response body");
                return BadGateway<BoricaSendConsentResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<BoricaSendConsentResult>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Sending consent by Borica returned null response body");
                return BadGateway<BoricaSendConsentResult>();
            }
            _logger.LogInformation("Sending consent by Borica completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to send consent by Borica. Request: POST {Url}", url);
            return UnhandledException<BoricaSendConsentResult>();
        }
    }

    public async Task<ServiceResult<BoricaCheckConsentStatusResponse>> CheckConsentAsync(string callbackId)
    {
        if (string.IsNullOrWhiteSpace(callbackId))
        {
            return BadRequest<BoricaCheckConsentStatusResponse>(nameof(callbackId), $"{nameof(callbackId)} is required.");
        }

        var url = $"{_applicationUrls.BoricaHostUrl}/consent/{callbackId}";
        _logger.LogInformation("Attempting to check consent by Borica online service.");

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
                                    "Failed checking consent by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            }));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Checking consent by Borica failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaCheckConsentStatusResponse>();
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Checking consent by Borica returned empty response body");
                return BadGateway<BoricaCheckConsentStatusResponse>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<BoricaCheckConsentStatusResponse>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Checking consent by Borica returned null response body");
                return BadGateway<BoricaCheckConsentStatusResponse>();
            }
            _logger.LogInformation("Checking consent by Borica completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to check consent by Borica. Request: GET {Url}", url);
            return UnhandledException<BoricaCheckConsentStatusResponse>();
        }
    }

    public async Task<ServiceResult<BoricaValidateConsentTokenResponse>> ValidateConsentTokenAsync(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return BadRequest<BoricaValidateConsentTokenResponse>(nameof(accessToken), $"{nameof(accessToken)} is required.");
        }

        var url = $"{_applicationUrls.BoricaHostUrl}/validate/token";
        _logger.LogInformation("Attempting to validate consent token by Borica online service.");

        try
        {
            var doNotRetryCodes = new System.Net.HttpStatusCode[] { System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.NotFound };
            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && !doNotRetryCodes.Contains(httpResponse.StatusCode))
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed validateing consent token by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() =>
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };
                request.Headers.Add("X-ACCESS-TOKEN", accessToken);
                return _httpClient.SendAsync(request);
            });
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Validating consent token by Borica failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<BoricaValidateConsentTokenResponse>();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var notFoundResponseObj = JsonConvert.DeserializeObject<IntegrationsApiResponse<BoricaValidateConsentTokenResponse>>(responseBody);
                    if (notFoundResponseObj?.Data is not null
                        && notFoundResponseObj.Data.ResponseCode == ResponseCode.AR_SIGN_TOKEN_NOT_FOUND)
                    {
                        _logger.LogWarning("Requested access token was not found in Borica.");
                        return NotFound<BoricaValidateConsentTokenResponse>(nameof(accessToken), notFoundResponseObj.Data.Message);
                    }
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Validating consent token by Borica returned empty response body");
                return BadGateway<BoricaValidateConsentTokenResponse>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<BoricaValidateConsentTokenResponse>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Validating consent token by Borica returned null response body");
                return BadGateway<BoricaValidateConsentTokenResponse>();
            }
            _logger.LogInformation("Validating consent token by Borica completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to validate consent token by Borica. Request: GET {Url}", url);
            return UnhandledException<BoricaValidateConsentTokenResponse>();
        }
    }

    public async Task<ServiceResult<ARSSyncSignedContentsResponse>> AutomaticRemoteSignDocumentAsync(BoricaARSSignDocument message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ARSBoricaSignDocumentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<ARSSyncSignedContentsResponse>(validationResult.Errors);
        }

        // Action
        var signingUrl = $"{_applicationUrls.BoricaHostUrl}/arsign/sync";
        var httpSignDocBody = BuildSignDocumentHttpBody(message);

        _logger.LogInformation("Attempting Automatic remote document sign by Borica online service.");

        try
        {
            var doNotRetryCodes = new System.Net.HttpStatusCode[] { System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.NotFound };

            var policy = Policy<HttpResponseMessage>
                            .Handle<Exception>()
                            .OrResult(httpResponse => !httpResponse.IsSuccessStatusCode && !doNotRetryCodes.Contains(httpResponse.StatusCode))
                            .WaitAndRetryAsync(3,
                                retryNumber => TimeSpan.FromSeconds(Math.Pow(retryNumber, 2)),
                            (exception, timespan) =>
                            {
                                _logger.LogWarning(
                                    exception.Exception,
                                    "Failed automatic remote document signing by Borica. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var accessToken = await _context.AccessTokens.FirstOrDefaultAsync(a => a.Status == AccessTokenStatus.Active);
            if (accessToken is null)
            {
                _logger.LogError("Active access token unavailable!");
                return UnhandledException<ARSSyncSignedContentsResponse>();
            }
            var response = await policy.ExecuteAsync(() =>
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(signingUrl),
                    Content = new StringContent(JsonConvert.SerializeObject(httpSignDocBody, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("X-ACCESS-TOKEN", accessToken.AccessTokenValue);
                return _httpClient.SendAsync(request);
            });

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Automatic remote document signing by Borica failed response: {Response}", responseBody);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<ARSSyncSignedContentsResponse>();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var notFoundResponseObj = JsonConvert.DeserializeObject<IntegrationsApiResponse<BoricaValidateConsentTokenResponse>>(responseBody);
                    if (notFoundResponseObj?.Data is not null
                        && notFoundResponseObj.Data.ResponseCode == ResponseCode.AR_SIGN_TOKEN_NOT_FOUND)
                    {
                        _logger.LogWarning("Passed access token was not found in Borica.");
                        return NotFound<ARSSyncSignedContentsResponse>(notFoundResponseObj.Data.ResponseCode.ToString(), notFoundResponseObj.Data.Message);
                    }
                }
            }

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation("Automatic remote document signing by Borica returned empty response body");
                return BadGateway<ARSSyncSignedContentsResponse>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<ARSSyncSignedContentsResponse>(responseBody);
            if (responseObj is null)
            {
                _logger.LogInformation("Automatic remote document signing by Borica returned null response body");
                return BadGateway<ARSSyncSignedContentsResponse>();
            }
            _logger.LogInformation("Automatic remote document signing by Borica completed successfully");

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to Automatic remote sign document by Borica. Request: POST {Url}, Body {Body}", signingUrl, httpSignDocBody);
            return UnhandledException<ARSSyncSignedContentsResponse>();
        }
    }
    public async Task<ServiceResult<IEnumerable<AccessTokenResult>>> GetAccessTokensAsync(BoricaGetAccessTokens message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var accessTokens = await _context.AccessTokens
            .AsNoTracking()
            .OrderByDescending(at => at.ExpirationDate)
            .ToListAsync();

        return Ok(accessTokens.AsEnumerable<AccessTokenResult>());
    }

    public async Task<ServiceResult<int>> AddAccessTokenAsync(BoricaAddAccessToken message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new BoricaAddAccessTokenValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<int>(validationResult.Errors);
        }

        // We want both changes to be executed in one transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.AccessTokens
                .Where(t => t.Status == AccessTokenStatus.Active)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(t => t.Status, AccessTokenStatus.Inactive)
                        .SetProperty(t => t.UpdatedAt, DateTime.UtcNow)
                );

            var newEntity = new Entity.AccessToken
            {
                CreatedAt = DateTime.UtcNow,
                AccessTokenValue = message.AccessTokenValue,
                ExpirationDate = message.ExpirationDate,
                Status = AccessTokenStatus.Active,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.AccessTokens.AddAsync(newEntity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Created(newEntity.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Exception during adding of access token.");
            return UnhandledException<int>();
        }
    }

    private static object BuildSignDocumentHttpBody(BoricaSignDocument docToSign)
    {
        var body = new
        {
            Contents = docToSign.Contents.Select(d => new { ConfirmText = d.ConfirmText, ContentFormat = d.ContentFormat, MediaType = d.MediaType, Data = d.Data, FileName = d.FileName, PadesVisualSignature = d.PadesVisualSignature, SignaturePosition = new { ImageHeight = d.SignaturePosition.ImageHeight, ImageWidth = d.SignaturePosition.ImageWidth, ImageXAxis = d.SignaturePosition.ImageXAxis, ImageYAxis = d.SignaturePosition.ImageYAxis, PageNumber = d.SignaturePosition.PageNumber } })
        };

        return body;
    }

    private static object BuildSignDocumentHttpBody(BoricaARSSignDocument docToSign)
    {
        var body = new
        {
            Contents = docToSign.Contents.Select(d => new
            {
                ConfirmText = d.ConfirmText,
                ContentFormat = d.ContentFormat,
                Data = d.Data,
                FileName = d.FileName,
                PadesVisualSignature = d.PadesVisualSignature,
                SignatureType = d.SignatureType
            })
        };

        return body;
    }
}

public class IntegrationsApiResponse<T>
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("error")]
    public string Error { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public string DataRaw { get; set; }

    [JsonIgnore]
    public T Data
    {
        get
        {
            if (string.IsNullOrEmpty(DataRaw))
                return default;

            return JsonConvert.DeserializeObject<T>(DataRaw);
        }
    }
}



