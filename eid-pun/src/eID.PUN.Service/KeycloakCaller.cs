﻿using eID.PUN.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.PUN.Service;

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
        _keyCloakHttpClient = httpClientFactory.CreateClient(HTTPClientName);
        _keyclockOptions = (keyclockOptions ?? throw new ArgumentNullException(nameof(keyclockOptions))).Value;
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
            response = await _keyCloakHttpClient.PostAsync(String.Format("/realms/{0}/protocol/openid-connect/token", _keyclockOptions.Realm), new FormUrlEncodedContent(data));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during getting token from Keycloak");
            return string.Empty;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed getting token from Keycloak");
            return string.Empty;
        }

        var responseStr = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseStr))
        {
            _logger.LogInformation("Recieved empty response from Keycloak");
            return string.Empty;
        }

        var keycloakAuthResponse = JsonConvert.DeserializeObject<KeycloakAuthResponse>(responseStr);
        if (keycloakAuthResponse is null)
        {
            _logger.LogInformation("Parsed null response from Keycloak");
            return string.Empty;
        }

        _logger.LogInformation("Successfully obtained Keycloak token");
        return keycloakAuthResponse.AccessToken;
    }
}

public class KeycloakAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonProperty("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    [JsonProperty("not-before-policy")]
    public int NotBeforePolicy { get; set; }
    [JsonProperty("session_state")]
    public string SessionState { get; set; }
    [JsonProperty("scope")]
    public string Scope { get; set; }
}


public interface IKeycloakCaller
{
    Task<string> GetTokenAsync();
}

