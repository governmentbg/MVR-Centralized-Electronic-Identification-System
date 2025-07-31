# IsceiSdk - PHP комплект със софтуерни компоненти за интеграция за ISCEI HTTP API


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



// Конфигуриране на Bearer (JWT) оторизация: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(    
    // Ако искате да използвате собствен http клиент, подайте ваш клиент, който имплементира `GuzzleHttp\ClientInterface`.
    // Това е по избор — по подразбиране ще се използва `GuzzleHttp\Client`.
    new GuzzleHttp\Client(),
    $config
);
$client_id = 'client_id_example'; // string
$approval_authentication_request_dto = new \IsceiSdk\Model\ApprovalAuthenticationRequestDto(); // \IsceiSdk\Model\ApprovalAuthenticationRequestDto
$scope = array('scope_example'); // string[]

try {
    $result = $apiInstance->approvalRequestAuth($client_id, $approval_authentication_request_dto, $scope);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling ApprovalRequestControllerApi->approvalRequestAuth: ', $e->getMessage(), PHP_EOL;
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://isceigwext.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*ApprovalRequestControllerApi* | [**approvalRequestAuth**](docs/Api/ApprovalRequestControllerApi.md#approvalrequestauth) | **POST** /api/v1/approval-request/auth/citizen | 
*ApprovalRequestControllerApi* | [**approvalRequestToken**](docs/Api/ApprovalRequestControllerApi.md#approvalrequesttoken) | **POST** /api/v1/approval-request/token | 
*ApprovalRequestControllerApi* | [**cibaRelyParty**](docs/Api/ApprovalRequestControllerApi.md#cibarelyparty) | **POST** /api/v1/approval-request/rely-party | 
*ApprovalRequestControllerApi* | [**evaluateRequestOutcome**](docs/Api/ApprovalRequestControllerApi.md#evaluaterequestoutcome) | **POST** /api/v1/approval-request/outcome | 
*ApprovalRequestControllerApi* | [**getUserApprovalRequests**](docs/Api/ApprovalRequestControllerApi.md#getuserapprovalrequests) | **GET** /api/v1/approval-request/user | 
*CommonLoginControllerApi* | [**associateEidWithCitizenProfile**](docs/Api/CommonLoginControllerApi.md#associateeidwithcitizenprofile) | **POST** /api/v1/auth/associate-profiles | 
*CommonLoginControllerApi* | [**basicLogin**](docs/Api/CommonLoginControllerApi.md#basiclogin) | **POST** /api/v1/auth/basic | 
*CommonLoginControllerApi* | [**generateAuthenticationChallenge**](docs/Api/CommonLoginControllerApi.md#generateauthenticationchallenge) | **POST** /api/v1/auth/generate-authentication-challenge | 
*CommonLoginControllerApi* | [**verifyOtp**](docs/Api/CommonLoginControllerApi.md#verifyotp) | **POST** /api/v1/auth/verify-otp | 
*MobileControllerApi* | [**mobileX509CertificateLogin**](docs/Api/MobileControllerApi.md#mobilex509certificatelogin) | **POST** /api/v1/auth/mobile/certificate-login | 
*StatisticsControllerApi* | [**reportDetailed**](docs/Api/StatisticsControllerApi.md#reportdetailed) | **GET** /api/v1/statistics/report/detailed | 
*StatisticsControllerApi* | [**reportRequestsCount**](docs/Api/StatisticsControllerApi.md#reportrequestscount) | **GET** /api/v1/statistics/report/requests-count | 
*StatisticsControllerApi* | [**reportRequestsTotal**](docs/Api/StatisticsControllerApi.md#reportrequeststotal) | **GET** /api/v1/statistics/report/requests-total | 
*X509CertificateCodeFlowControllerApi* | [**codeFlowAuth**](docs/Api/X509CertificateCodeFlowControllerApi.md#codeflowauth) | **POST** /api/v1/code-flow/auth | 
*X509CertificateCodeFlowControllerApi* | [**codeFlowToken**](docs/Api/X509CertificateCodeFlowControllerApi.md#codeflowtoken) | **GET** /api/v1/code-flow/token | 

## Модели

- [ApprovalAuthenticationRequestDto](docs/Model/ApprovalAuthenticationRequestDto.md)
- [ApprovalRequestResponse](docs/Model/ApprovalRequestResponse.md)
- [ApprovalRequestToken](docs/Model/ApprovalRequestToken.md)
- [AuthenticationRequestChallengeResponse](docs/Model/AuthenticationRequestChallengeResponse.md)
- [BasicLoginRequestDto](docs/Model/BasicLoginRequestDto.md)
- [MobileSignedChallenge](docs/Model/MobileSignedChallenge.md)
- [RelyPartyRequest](docs/Model/RelyPartyRequest.md)
- [RequestFromDto](docs/Model/RequestFromDto.md)
- [RequestOutcome](docs/Model/RequestOutcome.md)
- [SignedChallenge](docs/Model/SignedChallenge.md)
- [VerifyOtpDto](docs/Model/VerifyOtpDto.md)
- [X509CertAuthenticationRequestDto](docs/Model/X509CertAuthenticationRequestDto.md)

## Оторизация

Схеми за автентикация, дефинирани за API-то:
### ISCEI

- **Тип**: Bearer оторизация (JWT)

## Тестове

За да стартирате тестовете, използвайте:

```bash
composer install
vendor/bin/phpunit
```

## За този пакет

- API version: `1.0`
    - Generator version: `7.13.0`
- Build package: `org.openapitools.codegen.languages.PhpClientCodegen`
