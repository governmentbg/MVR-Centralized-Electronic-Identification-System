# PanSdk.Api.NotificationChannelsApi

All URIs are relative to *https://panapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiV1NotificationChannelsGet**](NotificationChannelsApi.md#apiv1notificationchannelsget) | **GET** /api/v1/NotificationChannels | Get all notification channels |
| [**ApiV1NotificationChannelsIdPut**](NotificationChannelsApi.md#apiv1notificationchannelsidput) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels. |
| [**ApiV1NotificationChannelsSelectedGet**](NotificationChannelsApi.md#apiv1notificationchannelsselectedget) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels |
| [**ApiV1NotificationChannelsSelectionPost**](NotificationChannelsApi.md#apiv1notificationchannelsselectionpost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels |
| [**RegisterAsync**](NotificationChannelsApi.md#registerasync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels. |

<a id="apiv1notificationchannelsget"></a>
# **ApiV1NotificationChannelsGet**
> UserNotificationChannelResultIPaginatedData ApiV1NotificationChannelsGet (int? pageSize = null, int? pageIndex = null, string channelName = null)

Get all notification channels

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class ApiV1NotificationChannelsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var pageSize = 100;  // int? |  (optional)  (default to 100)
            var pageIndex = 1;  // int? |  (optional)  (default to 1)
            var channelName = "channelName_example";  // string |  (optional) 

            try
            {
                // Get all notification channels
                UserNotificationChannelResultIPaginatedData result = apiInstance.ApiV1NotificationChannelsGet(pageSize, pageIndex, channelName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationChannelsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get all notification channels
    ApiResponse<UserNotificationChannelResultIPaginatedData> response = apiInstance.ApiV1NotificationChannelsGetWithHttpInfo(pageSize, pageIndex, channelName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageSize** | **int?** |  | [optional] [default to 100] |
| **pageIndex** | **int?** |  | [optional] [default to 1] |
| **channelName** | **string** |  | [optional]  |

### Return type

[**UserNotificationChannelResultIPaginatedData**](UserNotificationChannelResultIPaginatedData.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apiv1notificationchannelsidput"></a>
# **ApiV1NotificationChannelsIdPut**
> void ApiV1NotificationChannelsIdPut (Guid id, NotificationChannelPayload notificationChannelPayload = null)

Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class ApiV1NotificationChannelsIdPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var id = "id_example";  // Guid | 
            var notificationChannelPayload = new NotificationChannelPayload(); // NotificationChannelPayload |  (optional) 

            try
            {
                // Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.
                apiInstance.ApiV1NotificationChannelsIdPut(id, notificationChannelPayload);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsIdPut: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationChannelsIdPutWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.
    apiInstance.ApiV1NotificationChannelsIdPutWithHttpInfo(id, notificationChannelPayload);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsIdPutWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |
| **notificationChannelPayload** | [**NotificationChannelPayload**](NotificationChannelPayload.md) |  | [optional]  |

### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Success |  -  |
| **404** | Not Found |  -  |
| **409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apiv1notificationchannelsselectedget"></a>
# **ApiV1NotificationChannelsSelectedGet**
> GuidIPaginatedData ApiV1NotificationChannelsSelectedGet (int? pageSize = null, int? pageIndex = null)

Get user selected notification channels

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class ApiV1NotificationChannelsSelectedGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var pageSize = 1000;  // int? |  (optional)  (default to 1000)
            var pageIndex = 1;  // int? |  (optional)  (default to 1)

            try
            {
                // Get user selected notification channels
                GuidIPaginatedData result = apiInstance.ApiV1NotificationChannelsSelectedGet(pageSize, pageIndex);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsSelectedGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationChannelsSelectedGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get user selected notification channels
    ApiResponse<GuidIPaginatedData> response = apiInstance.ApiV1NotificationChannelsSelectedGetWithHttpInfo(pageSize, pageIndex);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsSelectedGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageSize** | **int?** |  | [optional] [default to 1000] |
| **pageIndex** | **int?** |  | [optional] [default to 1] |

### Return type

[**GuidIPaginatedData**](GuidIPaginatedData.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apiv1notificationchannelsselectionpost"></a>
# **ApiV1NotificationChannelsSelectionPost**
> void ApiV1NotificationChannelsSelectionPost (List<Guid> requestBody = null)

Register selection of user notification channels

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class ApiV1NotificationChannelsSelectionPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var requestBody = new List<Guid>(); // List<Guid> |  (optional) 

            try
            {
                // Register selection of user notification channels
                apiInstance.ApiV1NotificationChannelsSelectionPost(requestBody);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsSelectionPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationChannelsSelectionPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Register selection of user notification channels
    apiInstance.ApiV1NotificationChannelsSelectionPostWithHttpInfo(requestBody);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationChannelsApi.ApiV1NotificationChannelsSelectionPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **requestBody** | [**List&lt;Guid&gt;**](Guid.md) |  | [optional]  |

### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **204** | No Content |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="registerasync"></a>
# **RegisterAsync**
> Guid RegisterAsync (RegisterNotificationChannelRequest registerNotificationChannelRequest = null)

Add new notification channel in Pending table. Name must be unique against approved channels.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using PanSdk.Api;
using PanSdk.Client;
using PanSdk.Model;

namespace Example
{
    public class RegisterAsyncExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://panapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new NotificationChannelsApi(httpClient, config, httpClientHandler);
            var registerNotificationChannelRequest = new RegisterNotificationChannelRequest(); // RegisterNotificationChannelRequest |  (optional) 

            try
            {
                // Add new notification channel in Pending table. Name must be unique against approved channels.
                Guid result = apiInstance.RegisterAsync(registerNotificationChannelRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationChannelsApi.RegisterAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the RegisterAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Add new notification channel in Pending table. Name must be unique against approved channels.
    ApiResponse<Guid> response = apiInstance.RegisterAsyncWithHttpInfo(registerNotificationChannelRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationChannelsApi.RegisterAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **registerNotificationChannelRequest** | [**RegisterNotificationChannelRequest**](RegisterNotificationChannelRequest.md) |  | [optional]  |

### Return type

**Guid**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Returns the Id of the created notification channel. |  -  |
| **409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

