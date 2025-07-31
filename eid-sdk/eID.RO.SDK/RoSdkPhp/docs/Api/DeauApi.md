# RoSdk\DeauApi

All URIs are relative to https://roapipublic.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**approveEmpowermentByDeauAsync()**](DeauApi.md#approveEmpowermentByDeauAsync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment |
| [**denyEmpowermentByDeauAsync()**](DeauApi.md#denyEmpowermentByDeauAsync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments |
| [**getEmpowermentsByDeauAsync()**](DeauApi.md#getEmpowermentsByDeauAsync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data. |


## `approveEmpowermentByDeauAsync()`

```php
approveEmpowermentByDeauAsync($approve_empowerment_by_deau_request): string
```

Approve Unconfirmed empowerment

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = RoSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new RoSdk\Api\DeauApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
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

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **approve_empowerment_by_deau_request** | [**\RoSdk\Model\ApproveEmpowermentByDeauRequest**](../Model/ApproveEmpowermentByDeauRequest.md)|  | [optional] |

### Return type

**string**

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: `application/json-patch+json`, `application/json`, `text/json`, `application/*+json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `denyEmpowermentByDeauAsync()`

```php
denyEmpowermentByDeauAsync($deny_empowerment_by_deau_request): string
```

Deny both Active and Unconfirmed empowerments

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = RoSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new RoSdk\Api\DeauApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$deny_empowerment_by_deau_request = new \RoSdk\Model\DenyEmpowermentByDeauRequest(); // \RoSdk\Model\DenyEmpowermentByDeauRequest | 

try {
    $result = $apiInstance->denyEmpowermentByDeauAsync($deny_empowerment_by_deau_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling DeauApi->denyEmpowermentByDeauAsync: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **deny_empowerment_by_deau_request** | [**\RoSdk\Model\DenyEmpowermentByDeauRequest**](../Model/DenyEmpowermentByDeauRequest.md)|  | [optional] |

### Return type

**string**

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: `application/json-patch+json`, `application/json`, `text/json`, `application/*+json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `getEmpowermentsByDeauAsync()`

```php
getEmpowermentsByDeauAsync($get_empowerments_by_deau_request): \RoSdk\Model\EmpowermentStatementResultIPaginatedData
```

This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = RoSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new RoSdk\Api\DeauApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$get_empowerments_by_deau_request = new \RoSdk\Model\GetEmpowermentsByDeauRequest(); // \RoSdk\Model\GetEmpowermentsByDeauRequest | 

try {
    $result = $apiInstance->getEmpowermentsByDeauAsync($get_empowerments_by_deau_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling DeauApi->getEmpowermentsByDeauAsync: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **get_empowerments_by_deau_request** | [**\RoSdk\Model\GetEmpowermentsByDeauRequest**](../Model/GetEmpowermentsByDeauRequest.md)|  | [optional] |

### Return type

[**\RoSdk\Model\EmpowermentStatementResultIPaginatedData**](../Model/EmpowermentStatementResultIPaginatedData.md)

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: `application/json-patch+json`, `application/json`, `text/json`, `application/*+json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
