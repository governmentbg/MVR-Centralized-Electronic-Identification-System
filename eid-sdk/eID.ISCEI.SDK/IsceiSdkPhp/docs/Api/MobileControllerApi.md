# IsceiSdk\MobileControllerApi

All URIs are relative to https://isceigwext.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**mobileX509CertificateLogin()**](MobileControllerApi.md#mobileX509CertificateLogin) | **POST** /api/v1/auth/mobile/certificate-login |  |


## `mobileX509CertificateLogin()`

```php
mobileX509CertificateLogin($mobile_signed_challenge): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\MobileControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$mobile_signed_challenge = new \IsceiSdk\Model\MobileSignedChallenge(); // \IsceiSdk\Model\MobileSignedChallenge

try {
    $result = $apiInstance->mobileX509CertificateLogin($mobile_signed_challenge);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling MobileControllerApi->mobileX509CertificateLogin: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **mobile_signed_challenge** | [**\IsceiSdk\Model\MobileSignedChallenge**](../Model/MobileSignedChallenge.md)|  | |

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
