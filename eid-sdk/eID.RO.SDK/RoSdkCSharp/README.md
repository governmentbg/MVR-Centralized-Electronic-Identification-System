# RoSdk - C# комплект със софтуерни компоненти за интеграция за eID - RO HTTP API

Регистър на овластяванията (РО)

- API version: v1
- SDK version: 1.0.0
- Generator version: 7.13.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen

<a id="frameworks-supported"></a>
## Поддържани среди (Frameworks)
- .NET Core >=1.0
- .NET Framework >=4.6
- Mono/Xamarin >=vNext

<a id="dependencies"></a>
## Зависимости

- [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/) - 13.0.2 or later
- [JsonSubTypes](https://www.nuget.org/packages/JsonSubTypes/) - 1.8.0 or later
- [System.ComponentModel.Annotations](https://www.nuget.org/packages/System.ComponentModel.Annotations) - 5.0.0 or later

DLL файловете, включени в пакета, може да не са най-новата версия. Препоръчваме да използвате [NuGet](https://docs.nuget.org/consume/installing-nuget), за да получите най-новата версия на пакетите:
```
Install-Package Newtonsoft.Json
Install-Package JsonSubTypes
Install-Package System.ComponentModel.Annotations
```
<a id="installation"></a>
## Инсталация  
Генерирайте DLL файла с помощта на предпочитания от вас инструмент (напр. `dotnet build`)

След това включете DLL файла (от папката `bin`) в C# проекта и използвайте следните пространства от имена (namespaces):
```csharp
using RoSdk.Api;
using RoSdk.Client;
using RoSdk.Model;
```
<a id="usage"></a>
## Употреба

За да използвате API клиента с HTTP прокси, конфигурирайте `System.Net.WebProxy`
```csharp
Configuration c = new Configuration();
System.Net.WebProxy webProxy = new System.Net.WebProxy("http://myProxyUrl:80/");
webProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
c.Proxy = webProxy;
```

### Връзки  
Всеки ApiClass (по-точно ApiClient вътре в него) ще създаде инстанция на HttpClient. Тя ще се използва през целия жизнен цикъл и ще бъде освободена (dispose), когато се извика методът Dispose.

За по-добро управление на връзките е обичайна практика да се използват повторно инстанции на HttpClient и HttpClientHandler (виж [тук](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net) за подробности).  
За да използвате собствена инстанция на HttpClient, просто я подайте на конструктора на ApiClass.

```csharp
HttpClientHandler yourHandler = new HttpClientHandler();
HttpClient yourHttpClient = new HttpClient(yourHandler);
var api = new YourApiClass(yourHttpClient, yourHandler);
```

Ако искате да използвате HttpClient и нямате достъп до handler-а — например в контекст на Dependency Injection в Asp.net Core, когато използвате IHttpClientFactory.

```csharp
HttpClient yourHttpClient = new HttpClient();
var api = new YourApiClass(yourHttpClient);
```
Ще загубите някои конфигурационни настройки. Засегнатите функционалности са: задаване и получаване на бисквитки (Cookies), клиентски сертификати, прокси настройки. Трябва или ръчно да ги конфигурирате при настройката на HttpClient, или няма да бъдат достъпни.

Ето пример за DI конфигурация в примерен уеб проект:

```csharp
services.AddHttpClient<YourApiClass>(httpClient =>
   new PetApi(httpClient));
```


<a id="getting-started"></a>
## Първи стъпки

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using RoSdk.Api;
using RoSdk.Client;
using RoSdk.Model;

namespace Example
{
    public class Example
    {
        public static void Main()
        {

            Configuration config = new Configuration();
            config.BasePath = "https://roapipublic.sandbox.bgeid.bg";
            // Конфигуриране на Bearer токен за оторизация
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // създаване на инстанции на HttpClient и HttpClientHandler за повторна употреба по-късно с различни Api класове
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new DeauApi(httpClient, config, httpClientHandler);
            var approveEmpowermentByDeauRequest = new ApproveEmpowermentByDeauRequest(); // ApproveEmpowermentByDeauRequest |  (optional) 

            try
            {
                // Approve Unconfirmed empowerment
                Guid result = apiInstance.ApproveEmpowermentByDeauAsync(approveEmpowermentByDeauRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling DeauApi.ApproveEmpowermentByDeauAsync: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }

        }
    }
}
```

<a id="documentation-for-api-endpoints"></a>
## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://roapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*DeauApi* | [**ApproveEmpowermentByDeauAsync**](docs/DeauApi.md#approveempowermentbydeauasync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment
*DeauApi* | [**DenyEmpowermentByDeauAsync**](docs/DeauApi.md#denyempowermentbydeauasync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments
*DeauApi* | [**GetEmpowermentsByDeauAsync**](docs/DeauApi.md#getempowermentsbydeauasync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.


<a id="documentation-for-models"></a>
## Документация за моделите

 - [Model.AddEmpowermentStatementsRequest](docs/AddEmpowermentStatementsRequest.md)
 - [Model.ApproveEmpowermentByDeauRequest](docs/ApproveEmpowermentByDeauRequest.md)
 - [Model.AuthorizerIdentifierData](docs/AuthorizerIdentifierData.md)
 - [Model.CalculatedEmpowermentStatus](docs/CalculatedEmpowermentStatus.md)
 - [Model.DenyEmpowermentByDeauRequest](docs/DenyEmpowermentByDeauRequest.md)
 - [Model.DisagreeEmpowermentRequestPayload](docs/DisagreeEmpowermentRequestPayload.md)
 - [Model.EmpowermentDisagreementReasonResult](docs/EmpowermentDisagreementReasonResult.md)
 - [Model.EmpowermentDisagreementReasonTranslationResult](docs/EmpowermentDisagreementReasonTranslationResult.md)
 - [Model.EmpowermentDisagreementResult](docs/EmpowermentDisagreementResult.md)
 - [Model.EmpowermentSignatureResult](docs/EmpowermentSignatureResult.md)
 - [Model.EmpowermentStatementFromMeResult](docs/EmpowermentStatementFromMeResult.md)
 - [Model.EmpowermentStatementFromMeResultIPaginatedData](docs/EmpowermentStatementFromMeResultIPaginatedData.md)
 - [Model.EmpowermentStatementResult](docs/EmpowermentStatementResult.md)
 - [Model.EmpowermentStatementResultIPaginatedData](docs/EmpowermentStatementResultIPaginatedData.md)
 - [Model.EmpowermentStatementStatus](docs/EmpowermentStatementStatus.md)
 - [Model.EmpowermentWithdrawResult](docs/EmpowermentWithdrawResult.md)
 - [Model.EmpowermentWithdrawalReasonResult](docs/EmpowermentWithdrawalReasonResult.md)
 - [Model.EmpowermentWithdrawalReasonTranslationResult](docs/EmpowermentWithdrawalReasonTranslationResult.md)
 - [Model.EmpowermentWithdrawalStatus](docs/EmpowermentWithdrawalStatus.md)
 - [Model.EmpowermentsByDeauSortBy](docs/EmpowermentsByDeauSortBy.md)
 - [Model.EmpowermentsByEikFilterStatus](docs/EmpowermentsByEikFilterStatus.md)
 - [Model.EmpowermentsByEikSortBy](docs/EmpowermentsByEikSortBy.md)
 - [Model.EmpowermentsDenialReason](docs/EmpowermentsDenialReason.md)
 - [Model.EmpowermentsFromMeFilterStatus](docs/EmpowermentsFromMeFilterStatus.md)
 - [Model.EmpowermentsFromMeSortBy](docs/EmpowermentsFromMeSortBy.md)
 - [Model.EmpowermentsToMeFilterStatus](docs/EmpowermentsToMeFilterStatus.md)
 - [Model.EmpowermentsToMeSortBy](docs/EmpowermentsToMeSortBy.md)
 - [Model.GetEmpowermentsByDeauRequest](docs/GetEmpowermentsByDeauRequest.md)
 - [Model.GetEmpowermentsByEikFilterRequest](docs/GetEmpowermentsByEikFilterRequest.md)
 - [Model.GetEmpowermentsFromMeByFilterRequest](docs/GetEmpowermentsFromMeByFilterRequest.md)
 - [Model.IdentifierType](docs/IdentifierType.md)
 - [Model.LanguageType](docs/LanguageType.md)
 - [Model.OnBehalfOf](docs/OnBehalfOf.md)
 - [Model.ProblemDetails](docs/ProblemDetails.md)
 - [Model.ProviderDetailsStatus](docs/ProviderDetailsStatus.md)
 - [Model.ProviderListResult](docs/ProviderListResult.md)
 - [Model.ProviderListResultIPaginatedData](docs/ProviderListResultIPaginatedData.md)
 - [Model.ProviderServiceResult](docs/ProviderServiceResult.md)
 - [Model.ProviderServiceResultIPaginatedData](docs/ProviderServiceResultIPaginatedData.md)
 - [Model.ServiceScopeResult](docs/ServiceScopeResult.md)
 - [Model.SignEmpowermentPayload](docs/SignEmpowermentPayload.md)
 - [Model.SignatureProvider](docs/SignatureProvider.md)
 - [Model.SortDirection](docs/SortDirection.md)
 - [Model.StatusHistoryResult](docs/StatusHistoryResult.md)
 - [Model.TypeOfEmpowerment](docs/TypeOfEmpowerment.md)
 - [Model.UidAndUidTypeData](docs/UidAndUidTypeData.md)
 - [Model.UidResult](docs/UidResult.md)
 - [Model.UserIdentifierData](docs/UserIdentifierData.md)
 - [Model.ValidationProblemDetails](docs/ValidationProblemDetails.md)
 - [Model.VolumeOfRepresentationRequest](docs/VolumeOfRepresentationRequest.md)
 - [Model.VolumeOfRepresentationResult](docs/VolumeOfRepresentationResult.md)
 - [Model.WithdrawEmpowermentRequestPayload](docs/WithdrawEmpowermentRequestPayload.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията

Схеми за автентикация, дефинирани за API-то:
<a id="Bearer"></a>
### Bearer

- **Тип**: Bearer Authentication

