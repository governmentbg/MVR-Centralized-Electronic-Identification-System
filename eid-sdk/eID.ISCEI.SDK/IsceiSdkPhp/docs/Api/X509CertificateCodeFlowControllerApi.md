# IsceiSdk\X509CertificateCodeFlowControllerApi

All URIs are relative to https://isceigwext.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**codeFlowAuth()**](X509CertificateCodeFlowControllerApi.md#codeFlowAuth) | **POST** /api/v1/code-flow/auth |  |
| [**codeFlowToken()**](X509CertificateCodeFlowControllerApi.md#codeFlowToken) | **GET** /api/v1/code-flow/token |  |


## `codeFlowAuth()`

```php
codeFlowAuth($client_id, $response_type, $state, $redirect_uri, $code_challenge, $code_challenge_method, $signed_challenge, $scope): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\X509CertificateCodeFlowControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$client_id = 'client_id_example'; // string
$response_type = 'response_type_example'; // string
$state = 'state_example'; // string
$redirect_uri = 'redirect_uri_example'; // string
$code_challenge = 'code_challenge_example'; // string
$code_challenge_method = 'code_challenge_method_example'; // string
$signed_challenge = new \IsceiSdk\Model\SignedChallenge(); // \IsceiSdk\Model\SignedChallenge
$scope = array('scope_example'); // string[]

try {
    $result = $apiInstance->codeFlowAuth($client_id, $response_type, $state, $redirect_uri, $code_challenge, $code_challenge_method, $signed_challenge, $scope);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling X509CertificateCodeFlowControllerApi->codeFlowAuth: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **client_id** | **string**|  | |
| **response_type** | **string**|  | |
| **state** | **string**|  | |
| **redirect_uri** | **string**|  | |
| **code_challenge** | **string**|  | |
| **code_challenge_method** | **string**|  | |
| **signed_challenge** | [**\IsceiSdk\Model\SignedChallenge**](../Model/SignedChallenge.md)|  | |
| **scope** | [**string[]**](../Model/string.md)|  | [optional] |

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

## `codeFlowToken()`

```php
codeFlowToken($client_id, $grant_type, $code, $redirect_uri, $code_verifier, $refresh_token): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\X509CertificateCodeFlowControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$client_id = 'client_id_example'; // string
$grant_type = 'grant_type_example'; // string
$code = 'code_example'; // string
$redirect_uri = 'redirect_uri_example'; // string
$code_verifier = 'code_verifier_example'; // string
$refresh_token = 'refresh_token_example'; // string

try {
    $result = $apiInstance->codeFlowToken($client_id, $grant_type, $code, $redirect_uri, $code_verifier, $refresh_token);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling X509CertificateCodeFlowControllerApi->codeFlowToken: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **client_id** | **string**|  | |
| **grant_type** | **string**|  | |
| **code** | **string**|  | [optional] |
| **redirect_uri** | **string**|  | [optional] |
| **code_verifier** | **string**|  | [optional] |
| **refresh_token** | **string**|  | [optional] |

### Return type

**string**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
