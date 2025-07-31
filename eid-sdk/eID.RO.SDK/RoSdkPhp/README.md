# RoSdk - PHP комплект със софтуерни компоненти за интеграция за eID - RO HTTP API

Регистър на овластяванията (РО)


## Инсталация и употреба

### Изисквания

PHP 8.1 или по-нова версия.

### Composer

За да инсталирате обвързванията чрез [Composer](https://getcomposer.org/), добавете следното в `composer.json`:

```json
{
  "repositories": [
    {
      "type": "vcs",
      "url": "https://github.com/GIT_USER_ID/GIT_REPO_ID.git"
    }
  ],
  "require": {
    "GIT_USER_ID/GIT_REPO_ID": "*@dev"
  }
}
```

След това изпълнете `composer install`

### Ръчна инсталация

Изтеглете файловете и включете `autoload.php`:

```php
<?php
require_once('/path/to/OpenAPIClient-php/vendor/autoload.php');
```

## Първи стъпки

Моля, следвайте [процедурата по инсталация](#installation--usage) и след това изпълнете следното:

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');



// Конфигуриране на Bearer (JWT) оторизация
$config = RoSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new RoSdk\Api\DeauApi(
    // Ако искате да използвате собствен http клиент, подайте ваш клиент, който имплементира `GuzzleHttp\ClientInterface`.
    // Това е по избор — по подразбиране ще се използва `GuzzleHttp\Client`.
    new GuzzleHttp\Client(),
    $config
);
$approve_empowerment_by_deau_request = new \RoSdk\Model\ApproveEmpowermentByDeauRequest(); // \RoSdk\Model\ApproveEmpowermentByDeauRequest | 

try {
    $result = $apiInstance->approveEmpowermentByDeauAsync($approve_empowerment_by_deau_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling DeauApi->approveEmpowermentByDeauAsync: ', $e->getMessage(), PHP_EOL;
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://roapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*DeauApi* | [**approveEmpowermentByDeauAsync**](docs/Api/DeauApi.md#approveempowermentbydeauasync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment
*DeauApi* | [**denyEmpowermentByDeauAsync**](docs/Api/DeauApi.md#denyempowermentbydeauasync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments
*DeauApi* | [**getEmpowermentsByDeauAsync**](docs/Api/DeauApi.md#getempowermentsbydeauasync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.

## Модели

- [AddEmpowermentStatementsRequest](docs/Model/AddEmpowermentStatementsRequest.md)
- [ApproveEmpowermentByDeauRequest](docs/Model/ApproveEmpowermentByDeauRequest.md)
- [AuthorizerIdentifierData](docs/Model/AuthorizerIdentifierData.md)
- [CalculatedEmpowermentStatus](docs/Model/CalculatedEmpowermentStatus.md)
- [DenyEmpowermentByDeauRequest](docs/Model/DenyEmpowermentByDeauRequest.md)
- [DisagreeEmpowermentRequestPayload](docs/Model/DisagreeEmpowermentRequestPayload.md)
- [EmpowermentDisagreementReasonResult](docs/Model/EmpowermentDisagreementReasonResult.md)
- [EmpowermentDisagreementReasonTranslationResult](docs/Model/EmpowermentDisagreementReasonTranslationResult.md)
- [EmpowermentDisagreementResult](docs/Model/EmpowermentDisagreementResult.md)
- [EmpowermentSignatureResult](docs/Model/EmpowermentSignatureResult.md)
- [EmpowermentStatementFromMeResult](docs/Model/EmpowermentStatementFromMeResult.md)
- [EmpowermentStatementFromMeResultIPaginatedData](docs/Model/EmpowermentStatementFromMeResultIPaginatedData.md)
- [EmpowermentStatementResult](docs/Model/EmpowermentStatementResult.md)
- [EmpowermentStatementResultIPaginatedData](docs/Model/EmpowermentStatementResultIPaginatedData.md)
- [EmpowermentStatementStatus](docs/Model/EmpowermentStatementStatus.md)
- [EmpowermentWithdrawResult](docs/Model/EmpowermentWithdrawResult.md)
- [EmpowermentWithdrawalReasonResult](docs/Model/EmpowermentWithdrawalReasonResult.md)
- [EmpowermentWithdrawalReasonTranslationResult](docs/Model/EmpowermentWithdrawalReasonTranslationResult.md)
- [EmpowermentWithdrawalStatus](docs/Model/EmpowermentWithdrawalStatus.md)
- [EmpowermentsByDeauSortBy](docs/Model/EmpowermentsByDeauSortBy.md)
- [EmpowermentsByEikFilterStatus](docs/Model/EmpowermentsByEikFilterStatus.md)
- [EmpowermentsByEikSortBy](docs/Model/EmpowermentsByEikSortBy.md)
- [EmpowermentsDenialReason](docs/Model/EmpowermentsDenialReason.md)
- [EmpowermentsFromMeFilterStatus](docs/Model/EmpowermentsFromMeFilterStatus.md)
- [EmpowermentsFromMeSortBy](docs/Model/EmpowermentsFromMeSortBy.md)
- [EmpowermentsToMeFilterStatus](docs/Model/EmpowermentsToMeFilterStatus.md)
- [EmpowermentsToMeSortBy](docs/Model/EmpowermentsToMeSortBy.md)
- [GetEmpowermentsByDeauRequest](docs/Model/GetEmpowermentsByDeauRequest.md)
- [GetEmpowermentsByEikFilterRequest](docs/Model/GetEmpowermentsByEikFilterRequest.md)
- [GetEmpowermentsFromMeByFilterRequest](docs/Model/GetEmpowermentsFromMeByFilterRequest.md)
- [IdentifierType](docs/Model/IdentifierType.md)
- [LanguageType](docs/Model/LanguageType.md)
- [OnBehalfOf](docs/Model/OnBehalfOf.md)
- [ProblemDetails](docs/Model/ProblemDetails.md)
- [ProviderDetailsStatus](docs/Model/ProviderDetailsStatus.md)
- [ProviderListResult](docs/Model/ProviderListResult.md)
- [ProviderListResultIPaginatedData](docs/Model/ProviderListResultIPaginatedData.md)
- [ProviderServiceResult](docs/Model/ProviderServiceResult.md)
- [ProviderServiceResultIPaginatedData](docs/Model/ProviderServiceResultIPaginatedData.md)
- [ServiceScopeResult](docs/Model/ServiceScopeResult.md)
- [SignEmpowermentPayload](docs/Model/SignEmpowermentPayload.md)
- [SignatureProvider](docs/Model/SignatureProvider.md)
- [SortDirection](docs/Model/SortDirection.md)
- [StatusHistoryResult](docs/Model/StatusHistoryResult.md)
- [TypeOfEmpowerment](docs/Model/TypeOfEmpowerment.md)
- [UidAndUidTypeData](docs/Model/UidAndUidTypeData.md)
- [UidResult](docs/Model/UidResult.md)
- [UserIdentifierData](docs/Model/UserIdentifierData.md)
- [ValidationProblemDetails](docs/Model/ValidationProblemDetails.md)
- [VolumeOfRepresentationRequest](docs/Model/VolumeOfRepresentationRequest.md)
- [VolumeOfRepresentationResult](docs/Model/VolumeOfRepresentationResult.md)
- [WithdrawEmpowermentRequestPayload](docs/Model/WithdrawEmpowermentRequestPayload.md)

## Оторизация

Схеми за автентикация, дефинирани за API-то:
### Bearer

- **Тип**: Bearer authentication (JWT)

## Тестове

За да стартирате тестовете, използвайте:

```bash
composer install
vendor/bin/phpunit
```

## За този пакет


- API version: `v1`
    - Generator version: `7.13.0`
- Build package: `org.openapitools.codegen.languages.PhpClientCodegen`
