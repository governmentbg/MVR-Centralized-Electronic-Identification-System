using System.Text.RegularExpressions;
using eID.RO.Contracts.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.RO.Service;

public class MpozeiCaller : IMpozeiCaller
{
    public const string HTTPClientName = "Mpozei";

    private readonly ILogger<MpozeiCaller> _logger;
    private readonly HttpClient _mpozeiHttpClient;

    public MpozeiCaller(
        ILogger<MpozeiCaller> logger,
        IHttpClientFactory httpClientFactory)
    {
        if (httpClientFactory is null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mpozeiHttpClient = httpClientFactory.CreateClient(HTTPClientName);
    }

    public async Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid correlationId, string uId, IdentifierType identifierType)
    {
        if (Guid.Empty == correlationId)
        {
            throw new ArgumentException($"'{nameof(correlationId)}' cannot be empty.", nameof(correlationId));
        }
        if (string.IsNullOrWhiteSpace(uId))
        {
            throw new ArgumentException($"'{nameof(uId)}' cannot be null or whitespace.", nameof(uId));
        }

        var queryString = new Dictionary<string, string>()
        {
            { "number", uId },
            { "type", identifierType.ToString() }
        };
        HttpResponseMessage response;
        try
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/mpozei/api/v1/eidentities/find", queryString));
            requestMessage.Headers.TryAddWithoutValidation("Request-Id", correlationId.ToString());
            response = await _mpozeiHttpClient.SendAsync(requestMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during getting User profile from from Mpozei");
            return null;
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Mpozei return not successfull response. Response: {StatusCode} {Response}", response.StatusCode, responseStr);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new();
            }
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogWarning("Recieved empty response from Mpozei.");
            return null;
        }

        var userInfo = JsonConvert.DeserializeObject<MpozeiUserProfile>(responseStr);
        if (userInfo is null)
        {
            _logger.LogWarning("Parsed null response from Mpozei.");
            return null;
        }
        var maskedUid = Regex.Replace(uId, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        _logger.LogInformation("Successfully obtained User profile from Mpozei. Uid: {Uid}", maskedUid);
        return userInfo;
    }

    public async Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid eId)
    {
        if (eId == Guid.Empty)
        {
            _logger.LogWarning($"'{nameof(eId)}' cannot be null or whitespace.");
            return null;
        }

        HttpResponseMessage response;
        try
        {
            response = await _mpozeiHttpClient.GetAsync($"/mpozei/api/v1/eidentities/{eId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during getting User profile from from Mpozei");
            return null;
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Mpozei return not successfull response. Response: {StatusCode} {Response}", response.StatusCode, responseStr);
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogWarning("Recieved empty response from Mpozei");
            return null;
        }

        var userInfo = JsonConvert.DeserializeObject<MpozeiUserProfile>(responseStr);
        _logger.LogInformation("Successfully obtained User profile from Mpozei. EId: {Eid}", eId);
        return userInfo;
    }
}

public class MpozeiUserProfile
{
    public string EidentityId { get; set; } = string.Empty;
    public string CitizenProfileId { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CitizenIdentifierNumber { get; set; } = string.Empty;
    public string CitizenIdentifierType { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Language { get; set; } = "bg";
    public string FirebaseId { get; set; } = string.Empty;
}

public interface IMpozeiCaller
{
    Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid correlationId, string uId, IdentifierType identifierType);
    Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid eId);
}
