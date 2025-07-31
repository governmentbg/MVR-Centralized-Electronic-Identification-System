using System.Text;
using eID.Signing.API.Requests;
using eID.Signing.Application.Options;
using eID.Signing.Contracts.Commands;
using eID.Signing.Contracts.Results;
using eID.Signing.Service.Objects;
using eID.Signing.Service.Validators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;

namespace eID.Signing.Service;

public class EvrotrustSigningService : BaseService
{
    private readonly ILogger<EvrotrustSigningService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ApplicationUrls _applicationUrls;
    private readonly int _badRequestCode = 400;
    private readonly int _internalServerErrorCode = 500;

    public EvrotrustSigningService(
        ILogger<EvrotrustSigningService> logger,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IOptions<ApplicationUrls> applicationUrls)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient(ApplicationConstants.HttpClientWithMetricsName);
        _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
        _applicationUrls.Validate();
    }

    public async Task<ServiceResult<EvrotrustStatusResult>> GetFileStatusAsync(EvrotrustGetFileStatusByTransactionId message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new EvrotrustGetFileStatusByTransactyionIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<EvrotrustStatusResult>(validationResult.Errors);
        }

        // Action
        var getFileStatusUrl = $"{_applicationUrls.EvrotrustHostUrl}/document/status/{message.TransactionId}/{message.GroupSigning}";

        _logger.LogInformation("Attempting to get status for document signed by Evrotrust online service.");

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
                                    "Failed getting status for document signed by Evrotrust. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(getFileStatusUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed getting status for document signed by Evrotrust: {Response}", responseStr);

                var warningObj = JsonConvert.DeserializeObject<EvrotrustWarningResponse>(responseStr) ?? new EvrotrustWarningResponse();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<EvrotrustStatusResult>();
                }

                if ((warningObj.Status == _badRequestCode) || (warningObj.Status == _internalServerErrorCode))
                {
                    _logger.LogInformation("Getting status for document signed by Evrotrust warning response: {Message} - {Data}", warningObj.Message, warningObj.Data);

                    return BadRequest<EvrotrustStatusResult>(warningObj.Message, warningObj.Data);
                }
            }

            response.EnsureSuccessStatusCode();

            var statusResult = JsonConvert.DeserializeObject<EvrotrustStatusResult>(responseStr) ?? new EvrotrustStatusResult();

            _logger.LogInformation("Getting status for document signed by Evrotrust completed successfully");

            return Ok(statusResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to get status for document signed by Evrotrust. Request: GET {Url}", getFileStatusUrl);
            return UnhandledException<EvrotrustStatusResult>();
        }
    }

    public async Task<ServiceResult<IEnumerable<EvrotrustFileContent>>> DownloadFileAsync(EvrotrustDownloadFileByTransactionId message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new EvrotrustDownloadFileByTransactyionIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<EvrotrustFileContent>>(validationResult.Errors);
        }

        // Action
        var downloadFileUrl = $"{_applicationUrls.EvrotrustHostUrl}/document/download/{message.TransactionId}/{message.GroupSigning}";

        _logger.LogInformation("Attempting to download document signed by Evrotrust online service.");

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
                                    "Failed downloading document signed by Evrotrust. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(downloadFileUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed downloading document signed by Evrotrust: {Response}", responseStr);

                var warningObj = JsonConvert.DeserializeObject<EvrotrustWarningResponse>(responseStr) ?? new EvrotrustWarningResponse();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<IEnumerable<EvrotrustFileContent>>();
                }

                if ((warningObj.Status == _badRequestCode) || (warningObj.Status == _internalServerErrorCode))
                {
                    _logger.LogInformation("Downloading document signed by Evrotrust warning response: {Message} - {Data}", warningObj.Message, warningObj.Data);

                    return BadRequest<IEnumerable<EvrotrustFileContent>>(warningObj.Message, warningObj.Data);
                }
            }

            response.EnsureSuccessStatusCode();

            var documentResult = JsonConvert.DeserializeObject<IEnumerable<EvrotrustFileContent>>(responseStr, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }) ?? new List<EvrotrustFileContent>();

            _logger.LogInformation("Downloading document signed by Evrotrust completed successfully");

            return Ok(documentResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when downloading document signed by Evrotrust. Request: GET {Url}", downloadFileUrl);
            return UnhandledException<IEnumerable<EvrotrustFileContent>>();
        }
    }

    public async Task<ServiceResult<EvrotrustDocumentSigningResult>> SignDocumentAsync(EvrotrustSignDocument message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new EvrotrustSignDocumentValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<EvrotrustDocumentSigningResult>(validationResult.Errors);
        }

        // Action
        var signingUrl = $"{_applicationUrls.EvrotrustHostUrl}/document/sign";
        var httpSignDocBody = BuildSignDocumentHttpBody(message);

        _logger.LogInformation("Attempting document sign by Evrotrust online service.");

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
                                    "Failed document signing by Evrotrust. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
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
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Document signing by Evrotrust failed response: {Response}", responseStr);

                var warningObj = JsonConvert.DeserializeObject<EvrotrustWarningResponse>(responseStr) ?? new EvrotrustWarningResponse();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<EvrotrustDocumentSigningResult>();
                }

                if(warningObj.Status == _badRequestCode)
                {
                    _logger.LogInformation("Document signing by Evrotrust warning response: {Message} - {Data}", warningObj.Message, warningObj.Data);

                    return BadRequest<EvrotrustDocumentSigningResult>(warningObj.Message, warningObj.Data);
                }
                else if (warningObj.Status == _internalServerErrorCode)
                {
                    _logger.LogInformation("Document signing by Evrotrust error response: {Message} - {Data}", warningObj.Message, warningObj.Data);

                    return BadRequest<EvrotrustDocumentSigningResult>(warningObj.Message, warningObj.Data);
                }
            }

            if (string.IsNullOrEmpty(responseStr))
            {
                _logger.LogInformation("Document signing by Evrotrust returned empty response body");
                return BadGateway<EvrotrustDocumentSigningResult>();
            }

            response.EnsureSuccessStatusCode();
            var responseObj = JsonConvert.DeserializeObject<EvrotrustDocSigningResponse>(responseStr) ?? new EvrotrustDocSigningResponse();
            _logger.LogInformation("Document signing by Evrotrust completed successfully");

            var result = PrepareSigningResult(responseObj);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to sign document by Evrotrust. Request: POST {Url}, Body {Body}", signingUrl, httpSignDocBody);
            return UnhandledException<EvrotrustDocumentSigningResult>();
        }
    }

    public async Task<ServiceResult<EvrotrustUserCheckResult>> CheckUserAsync(EvrotrustCheckUserByUid message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new EvrotrustCheckUserByUidValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<EvrotrustUserCheckResult>(validationResult.Errors);
        }

        // Action
        var checkUserUrl = $"{_applicationUrls.EvrotrustHostUrl}/user/check/{message.Uid}";

        _logger.LogInformation("Attempting to check status for user by Uid in Evrotrust online service...");

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
                                    "Failed check status for user by Uid in Evrotrust. Status: ({StatusCode}) {Result}. Next attempt will be at {NextAttemptTime}",
                                    exception.Result?.StatusCode,
                                    exception.Result?.ToString(),
                                    DateTime.UtcNow.Add(timespan));
                            });

            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(checkUserUrl));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed check status for user by Uid in Evrotrust: {Response}", responseStr);

                var warningObj = JsonConvert.DeserializeObject<EvrotrustWarningResponse>(responseStr) ?? new EvrotrustWarningResponse();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest<EvrotrustUserCheckResult>();
                }

                if ((warningObj.Status == _badRequestCode) || (warningObj.Status == _internalServerErrorCode))
                {
                    _logger.LogInformation("Checking status for user by Uid in Evrotrust warning response: {Message} - {Data}", warningObj.Message, warningObj.Data);

                    return BadRequest<EvrotrustUserCheckResult>(warningObj.Message, warningObj.Data);
                }
            }

            response.EnsureSuccessStatusCode();

            var statusResult = JsonConvert.DeserializeObject<EvrotrustUserCheckResult>(responseStr) ?? new EvrotrustUserCheckResult();

            _logger.LogInformation("Checking status for user by Uid in Evrotrust completed successfully");

            return Ok(statusResult);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred when trying to check status for user by Uid in Evrotrust. Request: GET {Url}", checkUserUrl);
            return UnhandledException<EvrotrustUserCheckResult>();
        }
    }

    private static object BuildSignDocumentHttpBody(EvrotrustSignDocument docToSign)
    {
        var body = new
        {
            DateExpire = docToSign.DateExpire,
            Documents = docToSign.Documents.Select(d => new { Content = d.Content, FileName = d.FileName, ContentType = d.ContentType }),
            UserIdentifiers = docToSign.UserIdentifiers
        };

        return body;
    }

    private static EvrotrustDocumentSigningResult PrepareSigningResult(EvrotrustDocSigningResponse response)
    {
        var result = new EvrotrustDocumentSigningResult()
        {
            ThreadID = response.Response.ThreadID,
            Transactions = response.Response.Transactions.Select(t => new EvrotrustDocumentTransactionResult() { TransactionID = t.TransactionID, IdentificationNumber = t.IdentificationNumber }),
            GroupSigning = response.GroupSigning
        };

        return result;
    }
}


