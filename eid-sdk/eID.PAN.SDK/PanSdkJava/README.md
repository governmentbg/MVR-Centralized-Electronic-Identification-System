# PanSdk - Java комплект със софтуерни компоненти за интеграция за eID - PAN Public HTTP API

eID - PAN Public HTTP API
- API version: v1
  - Build date: 2025-06-11T13:49:58.753764600+03:00[Europe/Kiev]
  - Generator version: 7.13.0

Подсистема за автоматични нотификации (ПАН) Публична


## Изисквания

Създаването на клиентската библиотека за API изисква:
1. Java 1.8 или по-нова
2. Maven (3.8.3+) / Gradle (7.2+)

## Инсталация

За да инсталирате клиентската библиотека за API в локалното ви Maven хранилище, просто изпълнете:

```shell
mvn clean install
```

За да я публикувате в отдалечено Maven хранилище, конфигурирайте настройките на хранилището и изпълнете:

```shell
mvn clean deploy
```

Вижте [OSSRH ръководството](http://central.sonatype.org/pages/ossrh-guide.html) за повече информация.

### Maven потребители

Добавете тази зависимост към POM файла на вашия проект:

```xml
<dependency>
  <groupId>org.openapitools</groupId>
  <artifactId>openapi-java-client</artifactId>
  <version>v1</version>
  <scope>compile</scope>
</dependency>
```

### Gradle потребители

Добавете тази зависимост към build файла на вашия проект:

```groovy
  repositories {    
    mavenCentral()     // Необходимо, ако jar файлът 'openapi-java-client' е публикуван в Maven Central.
    mavenLocal()       // Необходимо, ако jar файлът 'openapi-java-client' е публикуван в локалното Maven хранилище.
  }

  dependencies {
     implementation "org.openapitools:openapi-java-client:v1"
  }
```

### Други

Първо генерирайте JAR файла, като изпълните:

```shell
mvn clean package
```

След това ръчно инсталирайте следните JAR файлове:

* `target/openapi-java-client-v1.jar`
* `target/lib/*.jar`

## Първи стъпки

Моля, следвайте инструкциите за [инсталация](#installation) и изпълнете следния Java код:

```java

// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import org.openapitools.client.model.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Конфигуриране на HTTP bearer оторизация: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    Integer pageSize = 100; // Integer | 
    Integer pageIndex = 1; // Integer | 
    String channelName = "channelName_example"; // String | 
    try {
      UserNotificationChannelResultIPaginatedData result = apiInstance.apiV1NotificationChannelsGet(pageSize, pageIndex, channelName);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#apiV1NotificationChannelsGet");
      System.err.println("Status code: " + e.getCode());
      System.err.println("Reason: " + e.getResponseBody());
      System.err.println("Response headers: " + e.getResponseHeaders());
      e.printStackTrace();
    }
  }
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://panapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*NotificationChannelsApi* | [**apiV1NotificationChannelsGet**](docs/NotificationChannelsApi.md#apiV1NotificationChannelsGet) | **GET** /api/v1/NotificationChannels | Get all notification channels
*NotificationChannelsApi* | [**apiV1NotificationChannelsIdPut**](docs/NotificationChannelsApi.md#apiV1NotificationChannelsIdPut) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.
*NotificationChannelsApi* | [**apiV1NotificationChannelsSelectedGet**](docs/NotificationChannelsApi.md#apiV1NotificationChannelsSelectedGet) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels
*NotificationChannelsApi* | [**apiV1NotificationChannelsSelectionPost**](docs/NotificationChannelsApi.md#apiV1NotificationChannelsSelectionPost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels
*NotificationChannelsApi* | [**registerAsync**](docs/NotificationChannelsApi.md#registerAsync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels.
*NotificationsApi* | [**apiV1NotificationsDeactivatePost**](docs/NotificationsApi.md#apiV1NotificationsDeactivatePost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events
*NotificationsApi* | [**apiV1NotificationsDeactivatedGet**](docs/NotificationsApi.md#apiV1NotificationsDeactivatedGet) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications
*NotificationsApi* | [**apiV1NotificationsGet**](docs/NotificationsApi.md#apiV1NotificationsGet) | **GET** /api/v1/Notifications | Get all Systems and notifications
*NotificationsApi* | [**registerOrUpdateSystemAsync**](docs/NotificationsApi.md#registerOrUpdateSystemAsync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events.
*NotificationsApi* | [**sendNotificationAsync**](docs/NotificationsApi.md#sendNotificationAsync) | **POST** /api/v1/Notifications/send | Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel.


## Документация за моделите

 - [GuidIPaginatedData](docs/GuidIPaginatedData.md)
 - [IdentifierType](docs/IdentifierType.md)
 - [NotificationChannelPayload](docs/NotificationChannelPayload.md)
 - [NotificationChannelTranslationRequest](docs/NotificationChannelTranslationRequest.md)
 - [NotificationChannelTranslationResult](docs/NotificationChannelTranslationResult.md)
 - [ProblemDetails](docs/ProblemDetails.md)
 - [RegisterNotificationChannelRequest](docs/RegisterNotificationChannelRequest.md)
 - [RegisterSystemRequest](docs/RegisterSystemRequest.md)
 - [RegisteredSystemResult](docs/RegisteredSystemResult.md)
 - [RegisteredSystemResultIPaginatedData](docs/RegisteredSystemResultIPaginatedData.md)
 - [RegisteredSystemTranslationRequest](docs/RegisteredSystemTranslationRequest.md)
 - [RegisteredSystemTranslationResult](docs/RegisteredSystemTranslationResult.md)
 - [SendNotificationRequestInput](docs/SendNotificationRequestInput.md)
 - [SystemEventRequest](docs/SystemEventRequest.md)
 - [SystemEventResult](docs/SystemEventResult.md)
 - [TranslationRequest](docs/TranslationRequest.md)
 - [TranslationResult](docs/TranslationResult.md)
 - [UserNotificationChannelResult](docs/UserNotificationChannelResult.md)
 - [UserNotificationChannelResultIPaginatedData](docs/UserNotificationChannelResultIPaginatedData.md)
 - [ValidationProblemDetails](docs/ValidationProblemDetails.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията


Схеми за автентикация, дефинирани за API-то:
<a id="Bearer"></a>
### Bearer

- **Тип**: HTTP Bearer Token оторизация (JWT)


## Препоръка

Препоръчително е да се създава отделна инстанция на `ApiClient` за всеки нишка (thread) в многонишкова среда, за да се избегнат потенциални проблеми.



