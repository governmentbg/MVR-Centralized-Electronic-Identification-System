using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RoSdk.Api;
using RoSdk.Api;
using RoSdk.Model;

namespace RoSdk.Example;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Authentication
        var authHttpClient = new HttpClient();
        var keyCloakUrl = "YOUR_KEYCLOAK_GET_TOKEN_URL";
        var authenticationSettings = new List<KeyValuePair<string, string>>
        {
            new("grant_type",""),
            new("username", ""),
            new("password",  ""),
            new("client_id", ""),
            new("client_secret", "")
        };
        var token = string.Empty;
        try
        {
            Console.WriteLine("Authenticating...");
            var authenticationResponse = await authHttpClient.PostAsync(keyCloakUrl, new FormUrlEncodedContent(authenticationSettings));
            authenticationResponse.EnsureSuccessStatusCode();
            var authenticationResponseStr = await authenticationResponse.Content.ReadAsStringAsync();
            var keycloakAuthResponse = JsonConvert.DeserializeObject<KeycloakAuthResponse>(authenticationResponseStr) ?? new KeycloakAuthResponse();
            token = keycloakAuthResponse.AccessToken;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        // API call
        var apiUrl = "API_URL";
        var httpClient = new HttpClient();
        // Authenticate
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Instantiating a client passing the previously configured http client
        var client = new DeauApi(httpClient, apiUrl);
        try
        {
            Console.WriteLine("Calling endpoint...");
            // The ID of the empowerment that needs to be approved
            var request = new ApproveEmpowermentByDeauRequest { EmpowermentId = Guid.Empty };
            var response = await client.ApproveEmpowermentByDeauAsyncAsync(request);

            var json = JsonConvert.SerializeObject(response);
            Console.WriteLine(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to EXIT");
        Console.ReadKey();
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
