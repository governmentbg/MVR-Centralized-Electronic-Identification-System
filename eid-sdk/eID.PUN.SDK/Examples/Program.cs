using System.Net.Http.Headers;
using eID.PUN.SDK.Clients;
using eID.PUN.SDK.Contracts;

public class Program
{
    static async Task Main(string[] args)
    {
        // Prepare HttpClient with baseUrl set to chosen PUN environment
        var punUrl = "http://localhost:60005";
        var httpClient = new HttpClient { BaseAddress = new Uri(punUrl) };

        // Authentication
        // There are two ways to authenticate your requests
        // 1. When making requests on behalf of agent token authentication should be used
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "Your token");
        // 2. Server to server communication relies on certificate authentication instead
        //httpClient.DefaultRequestHeaders.Add("X-EID-S2S-AUTH", clientCertificateAsBase64String);

        // Instantiating a client passing the previously configured http client
        var punClient = new PunClient(httpClient);

        // Register carrier request model with test data
        var registerCarrier = new RegisterCarrierRequest
        {
            SerialNumber = "TestSerialNumber",
            Type = "IdCard",
            CertificateId = new Guid("426a0567-8eb0-4e31-b384-42ffdc6753e5"),
            EId = new Guid("51a7b7bd-8709-4d7d-8a84-8a5f2579bce1"),
        };

        var response = await punClient.RegisterCarrierAsync(registerCarrier);

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
        Console.WriteLine(json);
        Console.ReadLine();
    }
}

