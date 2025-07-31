# Ръководство за програмиста
Описание как се използва SDK-тата за достъп до системата на eId

# Предварителна подготовка
За съответния програмен език който използвате във вашия проект, добавете:
- IsceiSdk добавя функционалност за отентикиране
- Sdk-то на модула с който искате да работите

# Стъпки за работа
## Authentication
### C#
```csharp
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
```
## Работа с модула
### C#
```csharp
        // Authenticate
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var apiUrl = "module-api-url";
        var client = new SomeModuleApi(httpClient, apiUrl);
        try
        {
            Console.WriteLine("Calling endpoint...");
            var request = new ...; // Prepare request if it needs
            var response = await client.SomeMethodAsync(request);

            var json = JsonConvert.SerializeObject(response);
            Console.WriteLine(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

```