using System.Net;
using System.Net.Mime;
using System.Text;
using eID.MIS.Contracts.EP.Enums;
using eID.MIS.Contracts.EP.External;
using eID.MIS.Contracts.EP.Results;
using eID.MIS.Contracts.EP.Validators;
using FluentValidation;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eID.MIS.Service;


public interface IEPaymentsCaller
{
    Task<CreatePaymentResponse> CreatePaymentAsync(RegisterPaymentRequest request);
    Task<GetPaymentStatusResponse> GetPaymentStatusAsync(string paymentRequestId);
    Task<BaseResponse> SuspendPaymentAsync(string paymentRequestId);
    Task<GetClientsByEikResponse> GetClientsByEikAsync(string eik);
}

public class EPaymentsCaller : IEPaymentsCaller
{
    public const string HTTPClientName = "Integrations";
    private readonly ILogger<EPaymentsCaller> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    public EPaymentsCaller(
        ILogger<EPaymentsCaller> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        if (httpClientFactory is null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClientFactory.CreateClient(HTTPClientName);
    }

    public async Task<CreatePaymentResponse> CreatePaymentAsync(RegisterPaymentRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var validator = new RegisterPaymentRequestValidator();
        validator.RuleFor(f => f.PaymentData.PaymentId).NotEmpty().MaximumLength(50);
        validator.RuleFor(f => f.PaymentData.ExpirationDate).NotEmpty();

        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new CreatePaymentResponse { HasFailed = true, Error = string.Join(",", validationResult.Errors) };
        }

        var EserviceClientId = _configuration.GetValue<string>("EPaymentsEServiceClientID");
        var stringContent = new StringContent(JsonConvert.SerializeObject(new { EserviceClientId = EserviceClientId, PaymentRequest = request }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, MediaTypeNames.Application.Json);
        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.PostAsync("ePayment/register-payment-extended", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new CreatePaymentResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EPayment request. {Response}", response?.ToString());
            return new CreatePaymentResponse { HasFailed = true, Error = "Failed getting EPayments response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<CreatePaymentResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EPayment response. RawResponse: {RawResponse}", body);
                return new CreatePaymentResponse { HasFailed = true, Error = "Failed deserializing EPayment response." };
            }
            if (!string.IsNullOrWhiteSpace(deserializedResponse.Error))
            {
                _logger.LogWarning("Error in EPayments response. RawResponse: {RawResponse}", body);
                return new CreatePaymentResponse { HasFailed = true, Error = "Error in EPayments response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EPayments response. {Response}", response?.ToString());
            return new CreatePaymentResponse { HasFailed = true, Error = "Failed parsing EPayments response." };
        }
    }

    public async Task<BaseResponse> SuspendPaymentAsync(string paymentRequestId)
    {
        if (string.IsNullOrWhiteSpace(paymentRequestId))
        {
            throw new ArgumentException($"'{nameof(paymentRequestId)}' cannot be null or whitespace.", nameof(paymentRequestId));
        }

        var request = new
        {
            PaymentsId = paymentRequestId,
            Status = PaymentStatusType.Suspended.ToString()
        };
        var stringContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, MediaTypeNames.Application.Json);
        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.PostAsync("ePayment/change-payment-status", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new BaseResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EPayment suspend payment request. {Response}", response?.ToString());
            return new BaseResponse { HasFailed = true, Error = "Failed getting EPayments suspend payment response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<CreatePaymentResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EPayment suspend payment response. {Response}", body);
                return new BaseResponse { HasFailed = true, Error = "Null EPayments suspend payment response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EPayments suspend payment response. {Response}", response?.ToString());
            return new BaseResponse { HasFailed = true, Error = "Failed parsing suspend payment response." };
        }
    }

    public async Task<GetPaymentStatusResponse> GetPaymentStatusAsync(string paymentRequestId)
    {
        if (string.IsNullOrWhiteSpace(paymentRequestId))
        {
            throw new ArgumentException($"'{nameof(paymentRequestId)}' cannot be null or whitespace.", nameof(paymentRequestId));
        }

        HttpResponseMessage response = null;
        try
        {
            var queryString = new Dictionary<string, string>()
            {
                { "paymentId", paymentRequestId}
            };
            var uri = QueryHelpers.AddQueryString("ePayment/payment-status", queryString);
            response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new GetPaymentStatusResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EPayment get payment status request. {Response}", response?.ToString());
            return new GetPaymentStatusResponse { HasFailed = true, Error = "Failed getting EPayments get payment status response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<GetPaymentStatusResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EPayment get payment status response. {Response}", response?.ToString());
                return new GetPaymentStatusResponse { HasFailed = true, Error = "Null EPayments get payment status response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EPayments get payment status response. {Response}", response?.ToString());
            return new GetPaymentStatusResponse { HasFailed = true, Error = "Failed parsing get payment status response." };
        }
    }

    public async Task<GetClientsByEikResponse> GetClientsByEikAsync(string eik)
    {
        if (string.IsNullOrWhiteSpace(eik))
        {
            throw new ArgumentException($"'{nameof(eik)}' cannot be null or whitespace.", nameof(eik));
        }

        HttpResponseMessage response = null;
        try
        {
            var queryString = new Dictionary<string, string>()
            {
                { "eik", eik }
            };
            var uri = QueryHelpers.AddQueryString("ePayment/get-clients-by-eik", queryString);
            response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new GetClientsByEikResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EPayment get clients by eik request. {Response}", response?.ToString());
            return new GetClientsByEikResponse { HasFailed = true, Error = "Failed getting EPayments get clients by eik response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<GetClientsByEikDTO>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EPayment get clients by eik response. {Response}; {ResponseBody}", response?.ToString(), body);
                return new GetClientsByEikResponse { HasFailed = true, Error = "Null EPayments get clients by eik response." };
            }
            return new GetClientsByEikResponse { Data = deserializedResponse };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EPayments get clients by eik response. {Response}", response?.ToString());
            return new GetClientsByEikResponse { HasFailed = true, Error = "Failed parsing get clients by eik response." };
        }
    }
}


public class CreatePaymentResponse : BaseResponse
{
    public string PaymentId { get; set; }
    public long RegistrationTime { get; set; }
    public string AccessCode { get; set; }
}

public class GetPaymentStatusResponse : BaseResponse
{
    public PaymentStatusType Status { get; set; }
    public long ChangeTime { get; set; }
}

public class GetClientsByEikResponse : BaseResponse
{
    public GetClientsByEikResult Data { get; set; }
}
