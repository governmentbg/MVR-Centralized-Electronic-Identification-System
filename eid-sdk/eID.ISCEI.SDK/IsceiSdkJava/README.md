# IsceiSdk - Java комплект със софтуерни компоненти за интеграция за ISCEI HTTP API

ISCEI HTTP API
- API version: 1.0
  - Build date: 2025-06-11T13:52:49.955870100+03:00[Europe/Kiev]
  - Generator version: 7.13.0

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
  <version>1.0</version>
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
     implementation "org.openapitools:openapi-java-client:1.0"
  }
```

### Други

Първо генерирайте JAR файла, като изпълните:

```shell
mvn clean package
```

След това ръчно инсталирайте следните JAR файлове:

* `target/openapi-java-client-1.0.jar`
* `target/lib/*.jar`

## Първи стъпки

Моля, следвайте инструкциите за [инсталация](#installation) и изпълнете следния Java код:

```java

// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import org.openapitools.client.model.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Конфигуриране на HTTP bearer оторизация: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    ApprovalAuthenticationRequestDto approvalAuthenticationRequestDto = new ApprovalAuthenticationRequestDto(); // ApprovalAuthenticationRequestDto | 
    Set<String> scope = Arrays.asList(); // Set<String> | 
    try {
      Object result = apiInstance.approvalRequestAuth(clientId, approvalAuthenticationRequestDto, scope);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#approvalRequestAuth");
      System.err.println("Status code: " + e.getCode());
      System.err.println("Reason: " + e.getResponseBody());
      System.err.println("Response headers: " + e.getResponseHeaders());
      e.printStackTrace();
    }
  }
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://isceigwext.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*ApprovalRequestControllerApi* | [**approvalRequestAuth**](docs/ApprovalRequestControllerApi.md#approvalRequestAuth) | **POST** /api/v1/approval-request/auth/citizen | 
*ApprovalRequestControllerApi* | [**approvalRequestToken**](docs/ApprovalRequestControllerApi.md#approvalRequestToken) | **POST** /api/v1/approval-request/token | 
*ApprovalRequestControllerApi* | [**cibaRelyParty**](docs/ApprovalRequestControllerApi.md#cibaRelyParty) | **POST** /api/v1/approval-request/rely-party | 
*ApprovalRequestControllerApi* | [**evaluateRequestOutcome**](docs/ApprovalRequestControllerApi.md#evaluateRequestOutcome) | **POST** /api/v1/approval-request/outcome | 
*ApprovalRequestControllerApi* | [**getUserApprovalRequests**](docs/ApprovalRequestControllerApi.md#getUserApprovalRequests) | **GET** /api/v1/approval-request/user | 
*CommonLoginControllerApi* | [**associateEidWithCitizenProfile**](docs/CommonLoginControllerApi.md#associateEidWithCitizenProfile) | **POST** /api/v1/auth/associate-profiles | 
*CommonLoginControllerApi* | [**basicLogin**](docs/CommonLoginControllerApi.md#basicLogin) | **POST** /api/v1/auth/basic | 
*CommonLoginControllerApi* | [**generateAuthenticationChallenge**](docs/CommonLoginControllerApi.md#generateAuthenticationChallenge) | **POST** /api/v1/auth/generate-authentication-challenge | 
*CommonLoginControllerApi* | [**verifyOtp**](docs/CommonLoginControllerApi.md#verifyOtp) | **POST** /api/v1/auth/verify-otp | 
*MobileControllerApi* | [**mobileX509CertificateLogin**](docs/MobileControllerApi.md#mobileX509CertificateLogin) | **POST** /api/v1/auth/mobile/certificate-login | 
*StatisticsControllerApi* | [**reportDetailed**](docs/StatisticsControllerApi.md#reportDetailed) | **GET** /api/v1/statistics/report/detailed | 
*StatisticsControllerApi* | [**reportRequestsCount**](docs/StatisticsControllerApi.md#reportRequestsCount) | **GET** /api/v1/statistics/report/requests-count | 
*StatisticsControllerApi* | [**reportRequestsTotal**](docs/StatisticsControllerApi.md#reportRequestsTotal) | **GET** /api/v1/statistics/report/requests-total | 
*X509CertificateCodeFlowControllerApi* | [**codeFlowAuth**](docs/X509CertificateCodeFlowControllerApi.md#codeFlowAuth) | **POST** /api/v1/code-flow/auth | 
*X509CertificateCodeFlowControllerApi* | [**codeFlowToken**](docs/X509CertificateCodeFlowControllerApi.md#codeFlowToken) | **GET** /api/v1/code-flow/token | 


## Документация за моделите

 - [ApprovalAuthenticationRequestDto](docs/ApprovalAuthenticationRequestDto.md)
 - [ApprovalRequestResponse](docs/ApprovalRequestResponse.md)
 - [ApprovalRequestToken](docs/ApprovalRequestToken.md)
 - [AuthenticationRequestChallengeResponse](docs/AuthenticationRequestChallengeResponse.md)
 - [BasicLoginRequestDto](docs/BasicLoginRequestDto.md)
 - [MobileSignedChallenge](docs/MobileSignedChallenge.md)
 - [RelyPartyRequest](docs/RelyPartyRequest.md)
 - [RequestFromDto](docs/RequestFromDto.md)
 - [RequestOutcome](docs/RequestOutcome.md)
 - [SignedChallenge](docs/SignedChallenge.md)
 - [VerifyOtpDto](docs/VerifyOtpDto.md)
 - [X509CertAuthenticationRequestDto](docs/X509CertAuthenticationRequestDto.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията

Схеми за автентикация, дефинирани за API-то:
<a id="ISCEI"></a>
### ISCEI

- **Тип**: HTTP Bearer Token оторизация (JWT)

## Препоръка

Препоръчително е да се създава отделна инстанция на `ApiClient` за всеки нишка (thread) в многонишкова среда, за да се избегнат потенциални проблеми.
