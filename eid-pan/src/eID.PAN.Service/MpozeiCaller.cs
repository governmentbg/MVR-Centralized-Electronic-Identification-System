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

    public async Task<MpozeiUserProfile> FetchUserProfileAsync(string uId, IdentifierType identifierType)
    {
        if (string.IsNullOrWhiteSpace(uId))
        {
            throw new ArgumentException($"'{nameof(uId)}' cannot be null or whitespace.", nameof(uId));
        }

        var keycloakToken = await _keycloakCaller.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(keycloakToken))
        {
            _logger.LogWarning("Unable to obtain Keycloak token");
            throw new InvalidOperationException("Unable to obtain Keycloak token");
        }

        var queryString = new Dictionary<string, string>()
        {
            { "number", uId },
            { "type", identifierType.ToString() }
        };

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
        var maskedUid = Regex.Replace(uId, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        _logger.LogInformation("Successfully obtained User profile from Mpozei. Uid: {Uid}", maskedUid);
        return userInfo;
    }

    public async Task<MpozeiUserProfile> FetchUserProfileAsync(Guid eId)
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
    public string EidentityId { get; set; }
    public string CitizenProfileId { get; set; }
    public bool Active { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string LastName { get; set; }
    public string CitizenIdentifierNumber { get; set; }
    public string CitizenIdentifierType { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Language { get; set; } = "bg";
    public List<string> RegistrationTokens { get; set; }
}

public interface IMpozeiCaller
{
    Task<MpozeiUserProfile> FetchUserProfileAsync(string uId, IdentifierType identifierType);
    Task<MpozeiUserProfile> FetchUserProfileAsync(Guid eId);
}
