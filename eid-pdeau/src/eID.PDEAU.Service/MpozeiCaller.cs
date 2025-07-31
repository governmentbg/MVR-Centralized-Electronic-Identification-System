using eID.PDEAU.Contracts.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eID.PDEAU.Service;

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

    public async Task<(bool ok, MpozeiUserProfile userProfile)> FetchUserProfileAsync(Guid correlationId, string uId, IdentifierType identifierType)
    {
        if (correlationId == Guid.Empty)
        {
            _logger.LogWarning($"'{nameof(correlationId)}' cannot be empty.");
            return (false, new MpozeiUserProfile());
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

        return await FetchUserProfileAsync(correlationId, QueryHelpers.AddQueryString("/mpozei/api/v1/eidentities/find", queryString));
    }

    public async Task<(bool ok, MpozeiUserProfile userProfile)> FetchUserProfileAsync(Guid correlationId, Guid eId)
    {
        if (correlationId == Guid.Empty)
        {
            _logger.LogWarning($"'{nameof(correlationId)}' cannot be empty.");
            return (false, new MpozeiUserProfile());
        }
        if (eId == Guid.Empty)
        {
            _logger.LogWarning($"'{nameof(eId)}' cannot be empty.");
            return (false, new MpozeiUserProfile());
        }

        return await FetchUserProfileAsync(correlationId, $"/mpozei/api/v1/eidentities/{eId}");
    }

    private async Task<(bool ok, MpozeiUserProfile userProfile)> FetchUserProfileAsync(Guid correlationId, string queryString)
    {
        HttpResponseMessage response;
        try
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, queryString);
            requestMessage.Headers.TryAddWithoutValidation(Contracts.Constants.HeaderNames.RequestId, correlationId.ToString());

            response = await _mpozeiHttpClient.SendAsync(requestMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during getting User profile from Mpozei");
            return (false, new MpozeiUserProfile());
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Mpozei return not successful response. Request {Request} Response: {StatusCode} {Response}", 
                queryString, response.StatusCode, response.ToString());
            return (false, new MpozeiUserProfile());
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogWarning("Received empty response from Mpozei.");
            return (false, new MpozeiUserProfile());
        }

        var userInfo = JsonConvert.DeserializeObject<MpozeiUserProfile>(responseStr);
        if (userInfo is null)
        {
            _logger.LogWarning("Parsed null response from Mpozei.");
            return (false, new MpozeiUserProfile());
        }
        _logger.LogInformation("Successfully obtained User profile from Mpozei.");

        return (true, userInfo);
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
    Task<(bool ok, MpozeiUserProfile userProfile)> FetchUserProfileAsync(Guid correlationId, string uId, IdentifierType identifierType);
    Task<(bool ok, MpozeiUserProfile userProfile)> FetchUserProfileAsync(Guid correlationId, Guid eId);
}
