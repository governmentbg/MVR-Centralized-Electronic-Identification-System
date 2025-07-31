# IsceiSdk\StatisticsControllerApi

All URIs are relative to https://isceigwext.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**reportDetailed()**](StatisticsControllerApi.md#reportDetailed) | **GET** /api/v1/statistics/report/detailed |  |
| [**reportRequestsCount()**](StatisticsControllerApi.md#reportRequestsCount) | **GET** /api/v1/statistics/report/requests-count |  |
| [**reportRequestsTotal()**](StatisticsControllerApi.md#reportRequestsTotal) | **GET** /api/v1/statistics/report/requests-total |  |


## `reportDetailed()`

```php
reportDetailed($create_date_from, $create_date_to, $system_type, $client_id, $is_employee, $success): string[][]
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\StatisticsControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$create_date_from = new \DateTime('2013-10-20T19:20:30+01:00'); // \DateTime
$create_date_to = new \DateTime('2013-10-20T19:20:30+01:00'); // \DateTime
$system_type = 'system_type_example'; // string
$client_id = 'client_id_example'; // string
$is_employee = True; // bool
$success = True; // bool

try {
    $result = $apiInstance->reportDetailed($create_date_from, $create_date_to, $system_type, $client_id, $is_employee, $success);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling StatisticsControllerApi->reportDetailed: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **create_date_from** | **\DateTime**|  | |
| **create_date_to** | **\DateTime**|  | |
| **system_type** | **string**|  | [optional] |
| **client_id** | **string**|  | [optional] |
| **is_employee** | **bool**|  | [optional] |
| **success** | **bool**|  | [optional] |

### Return type

**string[][]**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `reportRequestsCount()`

```php
reportRequestsCount($create_date_from, $create_date_to, $system_type, $client_id): string[][]
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\StatisticsControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$create_date_from = new \DateTime('2013-10-20T19:20:30+01:00'); // \DateTime
$create_date_to = new \DateTime('2013-10-20T19:20:30+01:00'); // \DateTime
$system_type = 'system_type_example'; // string
$client_id = 'client_id_example'; // string

try {
    $result = $apiInstance->reportRequestsCount($create_date_from, $create_date_to, $system_type, $client_id);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling StatisticsControllerApi->reportRequestsCount: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **create_date_from** | **\DateTime**|  | |
| **create_date_to** | **\DateTime**|  | |
| **system_type** | **string**|  | [optional] |
| **client_id** | **string**|  | [optional] |

### Return type

**string[][]**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `reportRequestsTotal()`

```php
reportRequestsTotal($year): string[][]
```



### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: ISCEI
$config = IsceiSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new IsceiSdk\Api\StatisticsControllerApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$year = 56; // int

try {
    $result = $apiInstance->reportRequestsTotal($year);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling StatisticsControllerApi->reportRequestsTotal: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **year** | **int**|  | |

### Return type

**string[][]**

### Authorization

[ISCEI](../../README.md#ISCEI)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
