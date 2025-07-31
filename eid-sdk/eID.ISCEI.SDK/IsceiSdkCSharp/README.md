# IsceiSdk - C# комплект със софтуерни компоненти за интеграция за ISCEI HTTP API

- API version: 1.0
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
using IsceiSdk.Api;
using IsceiSdk.Client;
using IsceiSdk.Model;
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
using IsceiSdk.Api;
using IsceiSdk.Client;
using IsceiSdk.Model;

namespace Example
{
    public class Example
    {
        public static void Main()
        {

            Configuration config = new Configuration();
            config.BasePath = "https://isceigwext.sandbox.bgeid.bg";
            // Конфигуриране на Bearer токен за оторизация: ISCEI
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // създаване на инстанции на HttpClient и HttpClientHandler за повторна употреба по-късно с различни Api класове
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var approvalAuthenticationRequestDto = new ApprovalAuthenticationRequestDto(); // ApprovalAuthenticationRequestDto | 
            var scope = new List<string>(); // List<string> |  (optional) 

            try
            {
                Object result = apiInstance.ApprovalRequestAuth(clientId, approvalAuthenticationRequestDto, scope);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.ApprovalRequestAuth: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }

        }
    }
}
```

<a id="documentation-for-api-endpoints"></a>
## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://isceigwext.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*ApprovalRequestControllerApi* | [**ApprovalRequestAuth**](docs/ApprovalRequestControllerApi.md#approvalrequestauth) | **POST** /api/v1/approval-request/auth/citizen | 
*ApprovalRequestControllerApi* | [**ApprovalRequestToken**](docs/ApprovalRequestControllerApi.md#approvalrequesttoken) | **POST** /api/v1/approval-request/token | 
*ApprovalRequestControllerApi* | [**CibaRelyParty**](docs/ApprovalRequestControllerApi.md#cibarelyparty) | **POST** /api/v1/approval-request/rely-party | 
*ApprovalRequestControllerApi* | [**EvaluateRequestOutcome**](docs/ApprovalRequestControllerApi.md#evaluaterequestoutcome) | **POST** /api/v1/approval-request/outcome | 
*ApprovalRequestControllerApi* | [**GetUserApprovalRequests**](docs/ApprovalRequestControllerApi.md#getuserapprovalrequests) | **GET** /api/v1/approval-request/user | 
*CommonLoginControllerApi* | [**AssociateEidWithCitizenProfile**](docs/CommonLoginControllerApi.md#associateeidwithcitizenprofile) | **POST** /api/v1/auth/associate-profiles | 
*CommonLoginControllerApi* | [**BasicLogin**](docs/CommonLoginControllerApi.md#basiclogin) | **POST** /api/v1/auth/basic | 
*CommonLoginControllerApi* | [**GenerateAuthenticationChallenge**](docs/CommonLoginControllerApi.md#generateauthenticationchallenge) | **POST** /api/v1/auth/generate-authentication-challenge | 
*CommonLoginControllerApi* | [**VerifyOtp**](docs/CommonLoginControllerApi.md#verifyotp) | **POST** /api/v1/auth/verify-otp | 
*MobileControllerApi* | [**MobileX509CertificateLogin**](docs/MobileControllerApi.md#mobilex509certificatelogin) | **POST** /api/v1/auth/mobile/certificate-login | 
*StatisticsControllerApi* | [**ReportDetailed**](docs/StatisticsControllerApi.md#reportdetailed) | **GET** /api/v1/statistics/report/detailed | 
*StatisticsControllerApi* | [**ReportRequestsCount**](docs/StatisticsControllerApi.md#reportrequestscount) | **GET** /api/v1/statistics/report/requests-count | 
*StatisticsControllerApi* | [**ReportRequestsTotal**](docs/StatisticsControllerApi.md#reportrequeststotal) | **GET** /api/v1/statistics/report/requests-total | 
*X509CertificateCodeFlowControllerApi* | [**CodeFlowAuth**](docs/X509CertificateCodeFlowControllerApi.md#codeflowauth) | **POST** /api/v1/code-flow/auth | 
*X509CertificateCodeFlowControllerApi* | [**CodeFlowToken**](docs/X509CertificateCodeFlowControllerApi.md#codeflowtoken) | **GET** /api/v1/code-flow/token | 


<a id="documentation-for-models"></a>
## Документация за моделите

 - [Model.ApprovalAuthenticationRequestDto](docs/ApprovalAuthenticationRequestDto.md)
 - [Model.ApprovalRequestResponse](docs/ApprovalRequestResponse.md)
 - [Model.ApprovalRequestToken](docs/ApprovalRequestToken.md)
 - [Model.AuthenticationRequestChallengeResponse](docs/AuthenticationRequestChallengeResponse.md)
 - [Model.BasicLoginRequestDto](docs/BasicLoginRequestDto.md)
 - [Model.MobileSignedChallenge](docs/MobileSignedChallenge.md)
 - [Model.RelyPartyRequest](docs/RelyPartyRequest.md)
 - [Model.RequestFromDto](docs/RequestFromDto.md)
 - [Model.RequestOutcome](docs/RequestOutcome.md)
 - [Model.SignedChallenge](docs/SignedChallenge.md)
 - [Model.VerifyOtpDto](docs/VerifyOtpDto.md)
 - [Model.X509CertAuthenticationRequestDto](docs/X509CertAuthenticationRequestDto.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията

Схеми за автентикация, дефинирани за API-то:
<a id="ISCEI"></a>
### ISCEI

- **Type**: Bearer Authentication

