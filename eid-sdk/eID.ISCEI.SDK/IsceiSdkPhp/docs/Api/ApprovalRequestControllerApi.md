# IsceiSdk\ApprovalRequestControllerApi

All URIs are relative to https://isceigwext.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**approvalRequestAuth()**](ApprovalRequestControllerApi.md#approvalRequestAuth) | **POST** /api/v1/approval-request/auth/citizen |  |
| [**approvalRequestToken()**](ApprovalRequestControllerApi.md#approvalRequestToken) | **POST** /api/v1/approval-request/token |  |
| [**cibaRelyParty()**](ApprovalRequestControllerApi.md#cibaRelyParty) | **POST** /api/v1/approval-request/rely-party |  |
| [**evaluateRequestOutcome()**](ApprovalRequestControllerApi.md#evaluateRequestOutcome) | **POST** /api/v1/approval-request/outcome |  |
| [**getUserApprovalRequests()**](ApprovalRequestControllerApi.md#getUserApprovalRequests) | **GET** /api/v1/approval-request/user |  |


## `approvalRequestAuth()`

```php
approvalRequestAuth($client_id, $approval_authentication_request_dto, $scope): object
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
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

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **client_id** | **string**|  | |
| **approval_authentication_request_dto** | [**\IsceiSdk\Model\ApprovalAuthenticationRequestDto**](../Model/ApprovalAuthenticationRequestDto.md)|  | |
| **scope** | [**string[]**](../Model/string.md)|  | [optional] |

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

## `approvalRequestToken()`

```php
approvalRequestToken($client_id, $approval_request_token): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$client_id = 'client_id_example'; // string
$approval_request_token = new \IsceiSdk\Model\ApprovalRequestToken(); // \IsceiSdk\Model\ApprovalRequestToken

try {
    $result = $apiInstance->approvalRequestToken($client_id, $approval_request_token);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling ApprovalRequestControllerApi->approvalRequestToken: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **client_id** | **string**|  | |
| **approval_request_token** | [**\IsceiSdk\Model\ApprovalRequestToken**](../Model/ApprovalRequestToken.md)|  | |

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

## `cibaRelyParty()`

```php
cibaRelyParty($rely_party_request): \DateTime
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$rely_party_request = new \IsceiSdk\Model\RelyPartyRequest(); // \IsceiSdk\Model\RelyPartyRequest

try {
    $result = $apiInstance->cibaRelyParty($rely_party_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling ApprovalRequestControllerApi->cibaRelyParty: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **rely_party_request** | [**\IsceiSdk\Model\RelyPartyRequest**](../Model/RelyPartyRequest.md)|  | |

### Return type

**\DateTime**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `*/*`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `evaluateRequestOutcome()`

```php
evaluateRequestOutcome($approval_request_id, $request_outcome): string
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$approval_request_id = 'approval_request_id_example'; // string
$request_outcome = new \IsceiSdk\Model\RequestOutcome(); // \IsceiSdk\Model\RequestOutcome

try {
    $result = $apiInstance->evaluateRequestOutcome($approval_request_id, $request_outcome);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling ApprovalRequestControllerApi->evaluateRequestOutcome: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **approval_request_id** | **string**|  | |
| **request_outcome** | [**\IsceiSdk\Model\RequestOutcome**](../Model/RequestOutcome.md)|  | |

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

## `getUserApprovalRequests()`

```php
getUserApprovalRequests(): \IsceiSdk\Model\ApprovalRequestResponse[]
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\ApprovalRequestControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);

try {
    $result = $apiInstance->getUserApprovalRequests();
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling ApprovalRequestControllerApi->getUserApprovalRequests: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

This endpoint does not need any parameter.

### Return type

[**\IsceiSdk\Model\ApprovalRequestResponse[]**](../Model/ApprovalRequestResponse.md)

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
