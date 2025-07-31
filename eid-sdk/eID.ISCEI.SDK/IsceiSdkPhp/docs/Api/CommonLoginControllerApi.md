# IsceiSdk\CommonLoginControllerApi

All URIs are relative to https://isceigwext.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**associateEidWithCitizenProfile()**](CommonLoginControllerApi.md#associateEidWithCitizenProfile) | **POST** /api/v1/auth/associate-profiles |  |
| [**basicLogin()**](CommonLoginControllerApi.md#basicLogin) | **POST** /api/v1/auth/basic |  |
| [**generateAuthenticationChallenge()**](CommonLoginControllerApi.md#generateAuthenticationChallenge) | **POST** /api/v1/auth/generate-authentication-challenge |  |
| [**verifyOtp()**](CommonLoginControllerApi.md#verifyOtp) | **POST** /api/v1/auth/verify-otp |  |


## `associateEidWithCitizenProfile()`

```php
associateEidWithCitizenProfile($client_id, $signed_challenge)
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\CommonLoginControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$client_id = 'client_id_example'; // string
$signed_challenge = new \IsceiSdk\Model\SignedChallenge(); // \IsceiSdk\Model\SignedChallenge

try {
    $apiInstance->associateEidWithCitizenProfile($client_id, $signed_challenge);
} catch (Exception $e) {
    echo 'Exception when calling CommonLoginControllerApi->associateEidWithCitizenProfile: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **client_id** | **string**|  | |
| **signed_challenge** | [**\IsceiSdk\Model\SignedChallenge**](../Model/SignedChallenge.md)|  | |

### Return type

void (empty response body)

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `basicLogin()`

```php
basicLogin($basic_login_request_dto): object
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\CommonLoginControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$basic_login_request_dto = new \IsceiSdk\Model\BasicLoginRequestDto(); // \IsceiSdk\Model\BasicLoginRequestDto

try {
    $result = $apiInstance->basicLogin($basic_login_request_dto);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling CommonLoginControllerApi->basicLogin: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **basic_login_request_dto** | [**\IsceiSdk\Model\BasicLoginRequestDto**](../Model/BasicLoginRequestDto.md)|  | |

### Return type

**object**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `generateAuthenticationChallenge()`

```php
generateAuthenticationChallenge($x509_cert_authentication_request_dto): \IsceiSdk\Model\AuthenticationRequestChallengeResponse
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\CommonLoginControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$x509_cert_authentication_request_dto = new \IsceiSdk\Model\X509CertAuthenticationRequestDto(); // \IsceiSdk\Model\X509CertAuthenticationRequestDto

try {
    $result = $apiInstance->generateAuthenticationChallenge($x509_cert_authentication_request_dto);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling CommonLoginControllerApi->generateAuthenticationChallenge: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **x509_cert_authentication_request_dto** | [**\IsceiSdk\Model\X509CertAuthenticationRequestDto**](../Model/X509CertAuthenticationRequestDto.md)|  | |

### Return type

[**\IsceiSdk\Model\AuthenticationRequestChallengeResponse**](../Model/AuthenticationRequestChallengeResponse.md)

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `verifyOtp()`

```php
verifyOtp($verify_otp_dto): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\CommonLoginControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$verify_otp_dto = new \IsceiSdk\Model\VerifyOtpDto(); // \IsceiSdk\Model\VerifyOtpDto

try {
    $result = $apiInstance->verifyOtp($verify_otp_dto);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling CommonLoginControllerApi->verifyOtp: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **verify_otp_dto** | [**\IsceiSdk\Model\VerifyOtpDto**](../Model/VerifyOtpDto.md)|  | |

### Return type

**string**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
