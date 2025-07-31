# PanSdk\NotificationChannelsApi

All URIs are relative to https://panapipublic.sandbox.bgeid.bg, except if the operation defines another base path.

| Method | HTTP request | Description |
| ------------- | ------------- | ------------- |
| [**apiV1NotificationChannelsGet()**](NotificationChannelsApi.md#apiV1NotificationChannelsGet) | **GET** /api/v1/NotificationChannels | Get all notification channels |
| [**apiV1NotificationChannelsIdPut()**](NotificationChannelsApi.md#apiV1NotificationChannelsIdPut) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels. |
| [**apiV1NotificationChannelsSelectedGet()**](NotificationChannelsApi.md#apiV1NotificationChannelsSelectedGet) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels |
| [**apiV1NotificationChannelsSelectionPost()**](NotificationChannelsApi.md#apiV1NotificationChannelsSelectionPost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels |
| [**registerAsync()**](NotificationChannelsApi.md#registerAsync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels. |


## `apiV1NotificationChannelsGet()`

```php
apiV1NotificationChannelsGet($page_size, $page_index, $channel_name): \PanSdk\Model\UserNotificationChannelResultIPaginatedData
```

Get all notification channels

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$page_size = 100; // int | 
$page_index = 1; // int | 
$channel_name = 'channel_name_example'; // string | 

try {
    $result = $apiInstance->apiV1NotificationChannelsGet($page_size, $page_index, $channel_name);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->apiV1NotificationChannelsGet: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **page_size** | **int**|  | [optional] [default to 100] |
| **page_index** | **int**|  | [optional] [default to 1] |
| **channel_name** | **string**|  | [optional] |

### Return type

[**\PanSdk\Model\UserNotificationChannelResultIPaginatedData**](../Model/UserNotificationChannelResultIPaginatedData.md)

### Authorization

[Bearer](../../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`

[[Back to top]](#) [[Back to API list]](../../README.md#endpoints)
[[Back to Model list]](../../README.md#models)
[[Back to README]](../../README.md)

## `apiV1NotificationChannelsIdPut()`

```php
apiV1NotificationChannelsIdPut($id, $notification_channel_payload)
```

Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$id = 'id_example'; // string | 
$notification_channel_payload = new \PanSdk\Model\NotificationChannelPayload(); // \PanSdk\Model\NotificationChannelPayload | 

try {
    $apiInstance->apiV1NotificationChannelsIdPut($id, $notification_channel_payload);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->apiV1NotificationChannelsIdPut: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **id** | **string**|  | |
| **notification_channel_payload** | [**\PanSdk\Model\NotificationChannelPayload**](../Model/NotificationChannelPayload.md)|  | [optional] |

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

## `apiV1NotificationChannelsSelectedGet()`

```php
apiV1NotificationChannelsSelectedGet($page_size, $page_index): \PanSdk\Model\GuidIPaginatedData
```

Get user selected notification channels

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$page_size = 1000; // int | 
$page_index = 1; // int | 

try {
    $result = $apiInstance->apiV1NotificationChannelsSelectedGet($page_size, $page_index);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->apiV1NotificationChannelsSelectedGet: ', $e->getMessage(), PHP_EOL;
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

## `apiV1NotificationChannelsSelectionPost()`

```php
apiV1NotificationChannelsSelectionPost($request_body)
```

Register selection of user notification channels

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$request_body = array('request_body_example'); // string[] | 

try {
    $apiInstance->apiV1NotificationChannelsSelectionPost($request_body);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->apiV1NotificationChannelsSelectionPost: ', $e->getMessage(), PHP_EOL;
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

## `registerAsync()`

```php
registerAsync($register_notification_channel_request): string
```

Add new notification channel in Pending table. Name must be unique against approved channels.

### Example

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');


// Configure Bearer (JWT) authorization: Bearer
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(
    // If you want use custom http client, pass your client which implements `GuzzleHttp\ClientInterface`.
    // This is optional, `GuzzleHttp\Client` will be used as default.
    new GuzzleHttp\Client(),
    $config
);
$register_notification_channel_request = new \PanSdk\Model\RegisterNotificationChannelRequest(); // \PanSdk\Model\RegisterNotificationChannelRequest | 

try {
    $result = $apiInstance->registerAsync($register_notification_channel_request);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->registerAsync: ', $e->getMessage(), PHP_EOL;
}
```

### Parameters

| Name | Type | Description  | Notes |
| ------------- | ------------- | ------------- | ------------- |
| **register_notification_channel_request** | [**\PanSdk\Model\RegisterNotificationChannelRequest**](../Model/RegisterNotificationChannelRequest.md)|  | [optional] |

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
