using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.Authorization.Keycloak;

public class KeycloakCaller : IKeycloakCaller
{
    public const string HTTPClientName = "Keycloak";
    private readonly ILogger<KeycloakCaller> _logger;
    private readonly HttpClient _keyCloakHttpClient;
    private readonly KeycloakOptions _keyclockOptions;

    public KeycloakCaller(
        ILogger<KeycloakCaller> logger, 
        IHttpClientFactory httpClientFactory,
        IOptions<KeycloakOptions> keyclockOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keyCloakHttpClient = httpClientFactory?.CreateClient(HTTPClientName) ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _keyclockOptions = (keyclockOptions ?? throw new ArgumentNullException(nameof(keyclockOptions))).Value;
        _keyclockOptions.Validate();
    }

    public async Task<string> GetTokenAsync()
    {
        _keyclockOptions.Validate();

        var data = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", _keyclockOptions.GrantType),
            new KeyValuePair<string, string>("username", _keyclockOptions.Username),
            new KeyValuePair<string, string>("password",  _keyclockOptions.Password),
            new KeyValuePair<string, string>("client_id", _keyclockOptions.ClientId),
            new KeyValuePair<string, string>("client_secret", _keyclockOptions.ClientSecret),
        };

        HttpResponseMessage response;
        try
        {
            response = await _keyCloakHttpClient.PostAsync(string.Format("/realms/{0}/protocol/openid-connect/token", _keyclockOptions.Realm), new FormUrlEncodedContent(data));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during getting token from Keycloak");
            return string.Empty;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed getting token from Keycloak. Response: {response}", response.ToString());
            return string.Empty;
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogWarning("Received empty response from Keycloak. Response: {response}", response.ToString());
            return string.Empty;
        }

        var keycloakAuthResponse = JsonConvert.DeserializeObject<KeycloakAuthResponse>(responseStr);
        if (keycloakAuthResponse is null)
        {
            _logger.LogWarning("Parsed null response from Keycloak. Content: {content}", responseStr);
            return string.Empty;
        }

        _logger.LogInformation("Successfully obtained Keycloak token");
        return keycloakAuthResponse.AccessToken;
    }
}

public class KeycloakAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonProperty("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;
    [JsonProperty("not-before-policy")]
    public int NotBeforePolicy { get; set; }
    [JsonProperty("session_state")]
    public string SessionState { get; set; } = string.Empty;
    [JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;
}

public interface IKeycloakCaller
{
    Task<string> GetTokenAsync();
}
