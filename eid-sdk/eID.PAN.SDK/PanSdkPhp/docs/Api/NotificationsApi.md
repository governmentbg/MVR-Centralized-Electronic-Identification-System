# PanSdk\NotificationsApi

All URIs are relative to https://panapipublic.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**apiV1NotificationsDeactivatePost()**](NotificationsApi.md#apiV1NotificationsDeactivatePost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events |
| [**apiV1NotificationsDeactivatedGet()**](NotificationsApi.md#apiV1NotificationsDeactivatedGet) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications |
| [**apiV1NotificationsGet()**](NotificationsApi.md#apiV1NotificationsGet) | **GET** /api/v1/Notifications | Get all Systems and notifications |
| [**registerOrUpdateSystemAsync()**](NotificationsApi.md#registerOrUpdateSystemAsync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events. |
| [**sendNotificationAsync()**](NotificationsApi.md#sendNotificationAsync) | **POST** /api/v1/Notifications/send | Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel. |


## `apiV1NotificationsDeactivatePost()`

```php
apiV1NotificationsDeactivatePost($request_body)
```

Register deactivated user events

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$request_body = array('request_body_example'); // string[] | 

try {
    $apiInstance->apiV1NotificationsDeactivatePost($request_body);
} catch (Exception $e) {
    echo 'Exception when calling NotificationsApi->apiV1NotificationsDeactivatePost: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **request_body** | [**string[]**](../Model/string.md)|  | [optional] |

### Return type

void (empty response body)

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: `application/json-patch+json`, `application/json`, `text/json`, `application/*+json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `apiV1NotificationsDeactivatedGet()`

```php
apiV1NotificationsDeactivatedGet($page_size, $page_index): \PanSdk\Model\GuidIPaginatedData
```

Get deactivated user notifications

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$page_size = 1000; // int | 
$page_index = 1; // int | 

try {
    $result = $apiInstance->apiV1NotificationsDeactivatedGet($page_size, $page_index);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationsApi->apiV1NotificationsDeactivatedGet: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **page_size** | **int**|  | [optional] [default to 1000] |
| **page_index** | **int**|  | [optional] [default to 1] |

### Return type

[**\PanSdk\Model\GuidIPaginatedData**](../Model/GuidIPaginatedData.md)

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `apiV1NotificationsGet()`

```php
apiV1NotificationsGet($page_size, $page_index, $system_name): \PanSdk\Model\RegisteredSystemResultIPaginatedData
```

Get all Systems and notifications

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$page_size = 50; // int | 
$page_index = 1; // int | 
$system_name = 'system_name_example'; // string | 

try {
    $result = $apiInstance->apiV1NotificationsGet($page_size, $page_index, $system_name);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationsApi->apiV1NotificationsGet: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **page_size** | **int**|  | [optional] [default to 50] |
| **page_index** | **int**|  | [optional] [default to 1] |
| **system_name** | **string**|  | [optional] |

### Return type

[**\PanSdk\Model\RegisteredSystemResultIPaginatedData**](../Model/RegisteredSystemResultIPaginatedData.md)

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `registerOrUpdateSystemAsync()`

```php
registerOrUpdateSystemAsync($register_system_request): string
```

Register or update a system with its events.

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$register_system_request = new \PanSdk\Model\RegisterSystemRequest(); // \PanSdk\Model\RegisterSystemRequest | 

try {
    $result = $apiInstance->registerOrUpdateSystemAsync($register_system_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationsApi->registerOrUpdateSystemAsync: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **register_system_request** | [**\PanSdk\Model\RegisterSystemRequest**](../Model/RegisterSystemRequest.md)|  | [optional] |

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

## `sendNotificationAsync()`

```php
sendNotificationAsync($send_notification_request_input): bool
```

Send notification to a user via users' selected channels or fallback to default(SMTP) channel.

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$send_notification_request_input = new \PanSdk\Model\SendNotificationRequestInput(); // \PanSdk\Model\SendNotificationRequestInput | 

try {
    $result = $apiInstance->sendNotificationAsync($send_notification_request_input);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationsApi->sendNotificationAsync: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **send_notification_request_input** | [**\PanSdk\Model\SendNotificationRequestInput**](../Model/SendNotificationRequestInput.md)|  | [optional] |

### Return type

**bool**

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: `application/json-patch+json`, `application/json`, `text/json`, `application/*+json`
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)
