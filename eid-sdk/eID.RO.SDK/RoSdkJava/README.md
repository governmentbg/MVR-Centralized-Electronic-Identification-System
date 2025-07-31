# RoSdkSdk - Java комплект със софтуерни компоненти за интеграция за eID - RO HTTP API

eID - RO HTTP API
- API version: v1
  - Build date: 2025-06-11T17:57:54.980305500+03:00[Europe/Kiev]
  - Generator version: 7.13.0

Регистър на овластяванията (РО)


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
import eid.sdk.ro.ApiClient;
import eid.sdk.ro.ApiException;
import eid.sdk.ro.Configuration;
import eid.sdk.ro.auth.*;
import org.openapitools.client.model.*;
import org.openapitools.client.api.DeauApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://roapipublic.sandbox.bgeid.bg");
    
    // Конфигуриране на HTTP bearer оторизация: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    DeauApi apiInstance = new DeauApi(defaultClient);
    ApproveEmpowermentByDeauRequest approveEmpowermentByDeauRequest = new ApproveEmpowermentByDeauRequest(); // ApproveEmpowermentByDeauRequest | 
    try {
      UUID result = apiInstance.approveEmpowermentByDeauAsync(approveEmpowermentByDeauRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling DeauApi#approveEmpowermentByDeauAsync");
      System.err.println("Status code: " + e.getCode());
      System.err.println("Reason: " + e.getResponseBody());
      System.err.println("Response headers: " + e.getResponseHeaders());
      e.printStackTrace();
    }
  }
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://roapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*DeauApi* | [**approveEmpowermentByDeauAsync**](docs/DeauApi.md#approveEmpowermentByDeauAsync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment
*DeauApi* | [**denyEmpowermentByDeauAsync**](docs/DeauApi.md#denyEmpowermentByDeauAsync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments
*DeauApi* | [**getEmpowermentsByDeauAsync**](docs/DeauApi.md#getEmpowermentsByDeauAsync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.


## Документация за моделите

 - [AddEmpowermentStatementsRequest](docs/AddEmpowermentStatementsRequest.md)
 - [ApproveEmpowermentByDeauRequest](docs/ApproveEmpowermentByDeauRequest.md)
 - [AuthorizerIdentifierData](docs/AuthorizerIdentifierData.md)
 - [CalculatedEmpowermentStatus](docs/CalculatedEmpowermentStatus.md)
 - [DenyEmpowermentByDeauRequest](docs/DenyEmpowermentByDeauRequest.md)
 - [DisagreeEmpowermentRequestPayload](docs/DisagreeEmpowermentRequestPayload.md)
 - [EmpowermentDisagreementReasonResult](docs/EmpowermentDisagreementReasonResult.md)
 - [EmpowermentDisagreementReasonTranslationResult](docs/EmpowermentDisagreementReasonTranslationResult.md)
 - [EmpowermentDisagreementResult](docs/EmpowermentDisagreementResult.md)
 - [EmpowermentSignatureResult](docs/EmpowermentSignatureResult.md)
 - [EmpowermentStatementFromMeResult](docs/EmpowermentStatementFromMeResult.md)
 - [EmpowermentStatementFromMeResultIPaginatedData](docs/EmpowermentStatementFromMeResultIPaginatedData.md)
 - [EmpowermentStatementResult](docs/EmpowermentStatementResult.md)
 - [EmpowermentStatementResultIPaginatedData](docs/EmpowermentStatementResultIPaginatedData.md)
 - [EmpowermentStatementStatus](docs/EmpowermentStatementStatus.md)
 - [EmpowermentWithdrawResult](docs/EmpowermentWithdrawResult.md)
 - [EmpowermentWithdrawalReasonResult](docs/EmpowermentWithdrawalReasonResult.md)
 - [EmpowermentWithdrawalReasonTranslationResult](docs/EmpowermentWithdrawalReasonTranslationResult.md)
 - [EmpowermentWithdrawalStatus](docs/EmpowermentWithdrawalStatus.md)
 - [EmpowermentsByDeauSortBy](docs/EmpowermentsByDeauSortBy.md)
 - [EmpowermentsByEikFilterStatus](docs/EmpowermentsByEikFilterStatus.md)
 - [EmpowermentsByEikSortBy](docs/EmpowermentsByEikSortBy.md)
 - [EmpowermentsDenialReason](docs/EmpowermentsDenialReason.md)
 - [EmpowermentsFromMeFilterStatus](docs/EmpowermentsFromMeFilterStatus.md)
 - [EmpowermentsFromMeSortBy](docs/EmpowermentsFromMeSortBy.md)
 - [EmpowermentsToMeFilterStatus](docs/EmpowermentsToMeFilterStatus.md)
 - [EmpowermentsToMeSortBy](docs/EmpowermentsToMeSortBy.md)
 - [GetEmpowermentsByDeauRequest](docs/GetEmpowermentsByDeauRequest.md)
 - [GetEmpowermentsByEikFilterRequest](docs/GetEmpowermentsByEikFilterRequest.md)
 - [GetEmpowermentsFromMeByFilterRequest](docs/GetEmpowermentsFromMeByFilterRequest.md)
 - [IdentifierType](docs/IdentifierType.md)
 - [LanguageType](docs/LanguageType.md)
 - [OnBehalfOf](docs/OnBehalfOf.md)
 - [ProblemDetails](docs/ProblemDetails.md)
 - [ProviderDetailsStatus](docs/ProviderDetailsStatus.md)
 - [ProviderListResult](docs/ProviderListResult.md)
 - [ProviderListResultIPaginatedData](docs/ProviderListResultIPaginatedData.md)
 - [ProviderServiceResult](docs/ProviderServiceResult.md)
 - [ProviderServiceResultIPaginatedData](docs/ProviderServiceResultIPaginatedData.md)
 - [ServiceScopeResult](docs/ServiceScopeResult.md)
 - [SignEmpowermentPayload](docs/SignEmpowermentPayload.md)
 - [SignatureProvider](docs/SignatureProvider.md)
 - [SortDirection](docs/SortDirection.md)
 - [StatusHistoryResult](docs/StatusHistoryResult.md)
 - [TypeOfEmpowerment](docs/TypeOfEmpowerment.md)
 - [UidAndUidTypeData](docs/UidAndUidTypeData.md)
 - [UidResult](docs/UidResult.md)
 - [UserIdentifierData](docs/UserIdentifierData.md)
 - [ValidationProblemDetails](docs/ValidationProblemDetails.md)
 - [VolumeOfRepresentationRequest](docs/VolumeOfRepresentationRequest.md)
 - [VolumeOfRepresentationResult](docs/VolumeOfRepresentationResult.md)
 - [WithdrawEmpowermentRequestPayload](docs/WithdrawEmpowermentRequestPayload.md)


<a id="documentation-for-authorization"></a>
## Документация за оторизацията


Схеми за автентикация, дефинирани за API-то:
<a id="Bearer"></a>
### Bearer

- **Тип**: HTTP Bearer Token оторизация (JWT)


## Препоръка

Препоръчително е да се създава отделна инстанция на `ApiClient` за всеки нишка (thread) в многонишкова среда, за да се избегнат потенциални проблеми.



