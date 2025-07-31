using System.Net.Http.Headers;
using IsceiSdk.Api;
using IsceiSdk.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PanSdk.Api;
using RoSdk.Api;
using RoSdk.Model;

namespace Demo;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Beautify json
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { IgnoreShouldSerializeMembers = true },
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include
        };

        // User authentication
        var authApiUrl = "iscei-api-url";
        var authHttpClient = new HttpClient();
        var authClient = new CommonLoginControllerApi(authHttpClient, authApiUrl);
        AuthResponse authResponse;
        try
        {
            Console.WriteLine("Authenticating...");
            var clientId = "client-id";
            var email = "user@email.com";
            var password = "user-password";
            var request = new BasicLoginRequestDto(clientId, email, password);
            var responseData = await authClient.BasicLoginAsync(request);

            authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData.ToString()) ?? new AuthResponse();
            Console.WriteLine("Authenticated");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return;
        }
        var token = authResponse.AccessToken;

        // API call
        var httpClient = new HttpClient();
        // Authenticate
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // RO
        var roApiUrl = "ro-api-url";
        var client = new DeauApi(httpClient, roApiUrl);
        try
        {
            Console.WriteLine("Calling RO endpoint...");
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

        // PAN
        var panApiUrl = "pan-api-url";
        var panClient = new NotificationChannelsApi(httpClient, panApiUrl);
        try
        {
            Console.WriteLine("Calling PAN endpoint...");
            var response = await panClient.ApiV1NotificationChannelsGetAsync(100, 1);

            var json = JsonConvert.SerializeObject(response);
            Console.WriteLine(response.ToJson());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
