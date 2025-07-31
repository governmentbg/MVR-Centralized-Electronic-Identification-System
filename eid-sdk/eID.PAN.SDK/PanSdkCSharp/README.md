# PanSdk - C# комплект със софтуерни компоненти за интеграция за eID - PAN Public HTTP API

Подсистема за автоматични нотификации (ПАН) Публична

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
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;
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
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class Example
    {
        public static void Main()
        {

            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Конфигуриране на Bearer токен за оторизация
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // създаване на инстанции на HttpClient и HttpClientHandler за повторна употреба по-късно с различни Api класове
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var pageSize = 100;  // int? |  (optional)  (default to 100)
            var pageIndex = 1;  // int? |  (optional)  (default to 1)
            var channelName = "channelName_example";  // string |  (optional) 

            try
            {
                // Get all notification channels
                UserNotificationChannelResultIPaginatedData result = apiInstance.ApiV1NotificationChannelsGet(pageSize, pageIndex, channelName);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsGet: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }

        }
    }
}
```

<a id="documentation-for-api-endpoints"></a>
## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://panapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*NotificationChannelsApi* | [**ApiV1NotificationChannelsGet**](docs/NotificationChannelsApi.md#apiv1notificationchannelsget) | **GET** /api/v1/NotificationChannels | Get all notification channels
*NotificationChannelsApi* | [**ApiV1NotificationChannelsIdPut**](docs/NotificationChannelsApi.md#apiv1notificationchannelsidput) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.
*NotificationChannelsApi* | [**ApiV1NotificationChannelsSelectedGet**](docs/NotificationChannelsApi.md#apiv1notificationchannelsselectedget) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels
*NotificationChannelsApi* | [**ApiV1NotificationChannelsSelectionPost**](docs/NotificationChannelsApi.md#apiv1notificationchannelsselectionpost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels
*NotificationChannelsApi* | [**RegisterAsync**](docs/NotificationChannelsApi.md#registerasync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels.
*NotificationsApi* | [**ApiV1NotificationsDeactivatePost**](docs/NotificationsApi.md#apiv1notificationsdeactivatepost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events
*NotificationsApi* | [**ApiV1NotificationsDeactivatedGet**](docs/NotificationsApi.md#apiv1notificationsdeactivatedget) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications
*NotificationsApi* | [**ApiV1NotificationsGet**](docs/NotificationsApi.md#apiv1notificationsget) | **GET** /api/v1/Notifications | Get all Systems and notifications
*NotificationsApi* | [**RegisterOrUpdateSystemAsync**](docs/NotificationsApi.md#registerorupdatesystemasync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events.
*NotificationsApi* | [**SendNotificationAsync**](docs/NotificationsApi.md#sendnotificationasync) | **POST** /api/v1/Notifications/send | Send notification to a user via users' selected channels or fallback to default(SMTP) channel.


<a id="documentation-for-models"></a>
## Документация за моделите

 - [Model.GuidIPaginatedData](docs/GuidIPaginatedData.md)
 - [Model.IdentifierType](docs/IdentifierType.md)
 - [Model.NotificationChannelPayload](docs/NotificationChannelPayload.md)
 - [Model.NotificationChannelTranslationRequest](docs/NotificationChannelTranslationRequest.md)
 - [Model.NotificationChannelTranslationResult](docs/NotificationChannelTranslationResult.md)
 - [Model.ProblemDetails](docs/ProblemDetails.md)
 - [Model.RegisterNotificationChannelRequest](docs/RegisterNotificationChannelRequest.md)
 - [Model.RegisterSystemRequest](docs/RegisterSystemRequest.md)
 - [Model.RegisteredSystemResult](docs/RegisteredSystemResult.md)
 - [Model.RegisteredSystemResultIPaginatedData](docs/RegisteredSystemResultIPaginatedData.md)
 - [Model.RegisteredSystemTranslationRequest](docs/RegisteredSystemTranslationRequest.md)
 - [Model.RegisteredSystemTranslationResult](docs/RegisteredSystemTranslationResult.md)
 - [Model.SendNotificationRequestInput](docs/SendNotificationRequestInput.md)
 - [Model.SystemEventRequest](docs/SystemEventRequest.md)
 - [Model.SystemEventResult](docs/SystemEventResult.md)
 - [Model.TranslationRequest](docs/TranslationRequest.md)
 - [Model.TranslationResult](docs/TranslationResult.md)
 - [Model.UserNotificationChannelResult](docs/UserNotificationChannelResult.md)
 - [Model.UserNotificationChannelResultIPaginatedData](docs/UserNotificationChannelResultIPaginatedData.md)
 - [Model.ValidationProblemDetails](docs/ValidationProblemDetails.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията

Схеми за автентикация, дефинирани за API-то:
<a id="ISCEI"></a>
### ISCEI

- **Тип**: Bearer Authentication


