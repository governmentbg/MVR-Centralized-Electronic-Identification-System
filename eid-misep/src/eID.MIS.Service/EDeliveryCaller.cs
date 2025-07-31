using System.Net.Mime;
using System.Text;
using eID.MIS.Contracts.SEV.External;
using eID.MIS.Contracts.SEV.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace eID.MIS.Service;


public interface IEDeliveryCaller
{
    Task<CreatePassiveIndividualProfileResponse> CreatePassiveIndividualProfileAsync(CreatePassiveIndividualProfileRequest request);
    Task<GetProfileResponse> GetUserProfileAsync(string profileId);
    Task<SearchProfileResponse> SearchUserProfileAsync(SearchUserProfileQuery query);
    Task<SendMessageResponse> SendMessageOnBehalfAsync(SendMessageOnBehalfRequest request);
    Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request);
}

public class EDeliveryCaller : IEDeliveryCaller
{
    public const string HTTPClientName = "Integrations";
    private readonly ILogger<EDeliveryCaller> _logger;
    private readonly HttpClient _httpClient;
    public EDeliveryCaller(
        ILogger<EDeliveryCaller> logger,
        IHttpClientFactory httpClientFactory)
    {
        if (httpClientFactory is null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient(HTTPClientName);
    }

    public async Task<CreatePassiveIndividualProfileResponse> CreatePassiveIndividualProfileAsync(CreatePassiveIndividualProfileRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var validator = new CreatePassiveIndividualProfileRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = string.Join(",", validationResult.Errors) };
        }

        var stringContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, MediaTypeNames.Application.Json);
        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.PostAsync("eDelivery/create-passive-individual-profile", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EDelivery request. {Response}", response?.ToString());
            return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = "Failed getting EDelivery response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<CreatePassiveIndividualProfileResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EDelivery response. RawResponse: {RawResponse}", body);
                return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = "Failed deserializing EDelivery response." };
            }
            if (!string.IsNullOrWhiteSpace(deserializedResponse.Error))
            {
                _logger.LogWarning("Error in EDelivery's response. RawResponse: {RawResponse}", body);
                return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = "Error in EDelivery response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EDelivery's response. {Response}", response?.ToString());
            return new CreatePassiveIndividualProfileResponse { HasFailed = true, Error = "Failed parsing EDelivery response." };
        }
    }

    public async Task<GetProfileResponse> GetUserProfileAsync(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
        {
            return new GetProfileResponse { HasFailed = true, Error = "ProfileId is required." };
        }

        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.GetAsync($"eDelivery/get-profile/{profileId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new GetProfileResponse { Error = "Profile not found." };
            }
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new GetProfileResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EDelivery request. {Response}", response?.ToString());
            return new GetProfileResponse { HasFailed = true, Error = "Failed getting EDelivery response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<GetProfileResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EDelivery response. RawResponse: {RawResponse}", response?.ToString());
                return new GetProfileResponse { HasFailed = true, Error = "Failed deserializing EDelivery response." };
            }
            if (!string.IsNullOrWhiteSpace(deserializedResponse.Error))
            {
                _logger.LogWarning("Error in EDelivery's response. RawResponse: {RawResponse}", response?.ToString());
                return new GetProfileResponse { HasFailed = true, Error = "Error in EDelivery response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EDelivery's response. {Response}", response?.ToString());
            return new GetProfileResponse { HasFailed = true, Error = "Failed parsing EDelivery response." };
        }
    }

    public async Task<SearchProfileResponse> SearchUserProfileAsync(SearchUserProfileQuery query)
    {
        var validator = new SearchUserProfileQueryValidator();
        var validationResult = validator.Validate(query);
        if (!validationResult.IsValid)
        {
            return new SearchProfileResponse { HasFailed = true, Error = string.Join(",", validationResult.Errors) };
        }

        HttpResponseMessage response = null;
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(query.Identifier), query.Identifier },
                { nameof(query.TargetGroupId), query.TargetGroupId }
            };

            var url = QueryHelpers.AddQueryString("eDelivery/search-profile", parameters);
            response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new SearchProfileResponse { Error = "Profile not found." };
            }
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new SearchProfileResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EDelivery request. {Response}", response?.ToString());
            return new SearchProfileResponse { HasFailed = true, Error = "Failed getting EDelivery response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<SearchProfileResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EDelivery response. RawResponse: {RawResponse}", response?.ToString());
                return new SearchProfileResponse { HasFailed = true, Error = "Failed deserializing EDelivery response." };
            }
            if (!string.IsNullOrWhiteSpace(deserializedResponse.Error))
            {
                _logger.LogWarning("Error in EDelivery's response. RawResponse: {RawResponse}", response?.ToString());
                return new SearchProfileResponse { HasFailed = true, Error = "Error in EDelivery response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EDelivery's response. {Response}", response?.ToString());
            return new SearchProfileResponse { HasFailed = true, Error = "Failed parsing EDelivery response." };
        }
    }

    public async Task<SendMessageResponse> SendMessageOnBehalfAsync(SendMessageOnBehalfRequest request)
    {
        var validator = new SendMessageOnBehalfRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new SendMessageResponse { HasFailed = true, Error = string.Join(",", validationResult.Errors) };
        }

        return await SendMessageImplAsync("eDelivery/send-message-on-behalf", JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }

    public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request)
    {
        var validator = new SendMessageRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new SendMessageResponse { HasFailed = true, Error = string.Join(",", validationResult.Errors) };
        }

        return await SendMessageImplAsync("eDelivery/send-message", JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }

    private async Task<SendMessageResponse> SendMessageImplAsync(string path, string serializedRequest)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
        }

        if (string.IsNullOrWhiteSpace(serializedRequest))
        {
            throw new ArgumentException($"'{nameof(serializedRequest)}' cannot be null or whitespace.", nameof(serializedRequest));
        }

        var stringContent = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeNames.Application.Json);
        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.PostAsync(path, stringContent);
            if (!response.IsSuccessStatusCode)
            {
                var integrationsResponse = JsonConvert.DeserializeObject<IntegrationsModuleRawResponse>(await response.Content.ReadAsStringAsync());
                var dataDetails = JsonConvert.DeserializeObject<DataDetails>(integrationsResponse?.Data ?? string.Empty);
                if (dataDetails?.Status == (int)HttpStatusCode.BadRequest)
                {
                    var error = string.Join(",", dataDetails.Errors.Values.SelectMany(err => err));
                    _logger.LogWarning("EDelivery Bad request. {Error}", error);
                    return new SendMessageResponse { HasFailed = true, Error = error };
                } 
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Cancellation could be caused by Socket timeout
            _logger.LogError(ex, "Failed making EDelivery request. {Response}", response?.ToString());
            return new SendMessageResponse { HasFailed = true, Error = "Failed getting EDelivery response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<SendMessageResponse>(body);
            if (deserializedResponse is null)
            {
                _logger.LogWarning("Failed deserializing EDelivery response. RawResponse: {RawResponse}", body);
                return new SendMessageResponse { HasFailed = true, Error = "Failed deserializing EDelivery response." };
            }
            if (!string.IsNullOrWhiteSpace(deserializedResponse.Error))
            {
                _logger.LogWarning("Error in EDelivery's response. RawResponse: {RawResponse}", body);
                return new SendMessageResponse { HasFailed = true, Error = "Error in EDelivery response." };
            }
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed parsing EDelivery's response. {Response}", response?.ToString());
            return new SendMessageResponse { HasFailed = true, Error = "Failed parsing EDelivery response." };
        }
    }
}

public class CreatePassiveIndividualProfileResponse : BaseResponse
{
    public string ProfileId { get; set; }
}

public class GetProfileResponse : BaseResponse
{
    public string ProfileId { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public AddressData Address { get; set; }
}
public class SearchProfileResponse : BaseResponse
{
    public string ProfileId { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
public class SendMessageResponse : BaseResponse
{
    public int Result { get; set; }
}
