using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using eID.PAN.Contracts.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.PAN.Service;

public class MpozeiCaller : IMpozeiCaller
{
    public const string HTTPClientName = "Mpozei";

    private readonly ILogger<MpozeiCaller> _logger;
    private readonly IKeycloakCaller _keycloakCaller;
    private readonly HttpClient _mpozeiHttpClient;

    public MpozeiCaller(
        ILogger<MpozeiCaller> logger,
        IHttpClientFactory httpClientFactory,
        IKeycloakCaller keycloakCaller)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keycloakCaller = keycloakCaller;
        _mpozeiHttpClient = httpClientFactory.CreateClient(HTTPClientName);
    }

    public async Task<MpozeiUserProfile?> FetchUserProfileAsync(string uId, IdentifierType identifierType)
    {
        if (string.IsNullOrWhiteSpace(uId))
        {
            throw new ArgumentException($"'{nameof(uId)}' cannot be null or whitespace.", nameof(uId));
        }

        var queryString = new Dictionary<string, string>()
        {
            { "number", uId },
            { "type", identifierType.ToString() }
        };
        var result = await FindEidentityAsync(queryString);
        var maskedUid = Regex.Replace(uId, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        _logger.LogInformation("Successfully obtained User profile from Mpozei for {UserParam}", maskedUid);
        return result;
    }

    public async Task<MpozeiUserProfile?> FetchUserProfileByCitizenProfileIdAsync(Guid citizenProfileId)
    {
        if (Guid.Empty == citizenProfileId)
        {
            throw new ArgumentException($"'{nameof(citizenProfileId)}' cannot be empty.", nameof(citizenProfileId));
        }

        var queryString = new Dictionary<string, string>()
        {
            { "citizenProfileId", citizenProfileId.ToString() }
        };
        var result = await FindEidentityAsync(queryString);
        _logger.LogInformation("Successfully obtained User profile from Mpozei for {UserParam}", citizenProfileId.ToString());
        return result;
    }

    private async Task<MpozeiUserProfile?> FindEidentityAsync(Dictionary<string, string> queryString)
    {
        if (queryString is null)
        {
            throw new ArgumentNullException(nameof(queryString));
        }

        var keycloakToken = await _keycloakCaller.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(keycloakToken))
        {
            _logger.LogWarning("Unable to obtain Keycloak token");
            throw new InvalidOperationException("Unable to obtain Keycloak token");
        }

        _mpozeiHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakToken);
        HttpResponseMessage response;
        try
        {
            response = await _mpozeiHttpClient.GetAsync(QueryHelpers.AddQueryString("/mpozei/api/v1/eidentities/find", queryString));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during getting User profile from from Mpozei");
            return null;
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Mpozei return not successfull response. Response: {Response}", responseStr);
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogInformation("Recieved empty response from Mpozei.");
            return null;
        }

        var userInfo = JsonConvert.DeserializeObject<MpozeiUserProfile>(responseStr);
        if (userInfo is null)
        {
            _logger.LogInformation("Parsed null response from Mpozei.");
            return null;
        }
        return userInfo;
    }

    public async Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid eId)
    {
        if (eId == Guid.Empty)
        {
            _logger.LogWarning($"'{nameof(eId)}' cannot be null or whitespace.");
            return null;
        }

        var keycloakToken = await _keycloakCaller.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(keycloakToken))
        {
            _logger.LogWarning("Unable to obtain Keycloak token");
            return null;
        }

        _mpozeiHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakToken);
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
            _logger.LogWarning("Mpozei return not successfull response. Response: {Response}", responseStr);
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogInformation("Recieved empty response from Mpozei");
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
    Task<MpozeiUserProfile?> FetchUserProfileAsync(string uId, IdentifierType identifierType);
    Task<MpozeiUserProfile?> FetchUserProfileAsync(Guid eId);
    Task<MpozeiUserProfile?> FetchUserProfileByCitizenProfileIdAsync(Guid citizenProfileId);
}
