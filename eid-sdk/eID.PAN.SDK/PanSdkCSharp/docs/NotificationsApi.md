# PanSdk.Api.NotificationsApi

All URIs are relative to *https://panapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiV1NotificationsDeactivatePost**](NotificationsApi.md#apiv1notificationsdeactivatepost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events |
| [**ApiV1NotificationsDeactivatedGet**](NotificationsApi.md#apiv1notificationsdeactivatedget) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications |
| [**ApiV1NotificationsGet**](NotificationsApi.md#apiv1notificationsget) | **GET** /api/v1/Notifications | Get all Systems and notifications |
| [**RegisterOrUpdateSystemAsync**](NotificationsApi.md#registerorupdatesystemasync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events. |
| [**SendNotificationAsync**](NotificationsApi.md#sendnotificationasync) | **POST** /api/v1/Notifications/send | Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel. |

<a id="apiv1notificationsdeactivatepost"></a>
# **ApiV1NotificationsDeactivatePost**
> void ApiV1NotificationsDeactivatePost (List<Guid> requestBody = null)

Register deactivated user events

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
    public class ApiV1NotificationsDeactivatePostExample
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
            var apiInstance = new NotificationsApi(httpClient, config, httpClientHandler);
            var requestBody = new List<Guid>(); // List<Guid> |  (optional) 

            try
            {
                // Register deactivated user events
                apiInstance.ApiV1NotificationsDeactivatePost(requestBody);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsDeactivatePost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationsDeactivatePostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Register deactivated user events
    apiInstance.ApiV1NotificationsDeactivatePostWithHttpInfo(requestBody);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsDeactivatePostWithHttpInfo: " + e.Message);
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

<a id="apiv1notificationsdeactivatedget"></a>
# **ApiV1NotificationsDeactivatedGet**
> GuidIPaginatedData ApiV1NotificationsDeactivatedGet (int? pageSize = null, int? pageIndex = null)

Get deactivated user notifications

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
    public class ApiV1NotificationsDeactivatedGetExample
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
            var apiInstance = new NotificationsApi(httpClient, config, httpClientHandler);
            var pageSize = 1000;  // int? |  (optional)  (default to 1000)
            var pageIndex = 1;  // int? |  (optional)  (default to 1)

            try
            {
                // Get deactivated user notifications
                GuidIPaginatedData result = apiInstance.ApiV1NotificationsDeactivatedGet(pageSize, pageIndex);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsDeactivatedGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationsDeactivatedGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get deactivated user notifications
    ApiResponse<GuidIPaginatedData> response = apiInstance.ApiV1NotificationsDeactivatedGetWithHttpInfo(pageSize, pageIndex);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsDeactivatedGetWithHttpInfo: " + e.Message);
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

<a id="apiv1notificationsget"></a>
# **ApiV1NotificationsGet**
> RegisteredSystemResultIPaginatedData ApiV1NotificationsGet (int? pageSize = null, int? pageIndex = null, string systemName = null)

Get all Systems and notifications

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
    public class ApiV1NotificationsGetExample
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
            var apiInstance = new NotificationsApi(httpClient, config, httpClientHandler);
            var pageSize = 50;  // int? |  (optional)  (default to 50)
            var pageIndex = 1;  // int? |  (optional)  (default to 1)
            var systemName = "systemName_example";  // string |  (optional) 

            try
            {
                // Get all Systems and notifications
                RegisteredSystemResultIPaginatedData result = apiInstance.ApiV1NotificationsGet(pageSize, pageIndex, systemName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiV1NotificationsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Get all Systems and notifications
    ApiResponse<RegisteredSystemResultIPaginatedData> response = apiInstance.ApiV1NotificationsGetWithHttpInfo(pageSize, pageIndex, systemName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationsApi.ApiV1NotificationsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageSize** | **int?** |  | [optional] [default to 50] |
| **pageIndex** | **int?** |  | [optional] [default to 1] |
| **systemName** | **string** |  | [optional]  |

### Return type

[**RegisteredSystemResultIPaginatedData**](RegisteredSystemResultIPaginatedData.md)

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

<a id="registerorupdatesystemasync"></a>
# **RegisterOrUpdateSystemAsync**
> Guid RegisterOrUpdateSystemAsync (RegisterSystemRequest registerSystemRequest = null)

Register or update a system with its events.

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
    public class RegisterOrUpdateSystemAsyncExample
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
            var apiInstance = new NotificationsApi(httpClient, config, httpClientHandler);
            var registerSystemRequest = new RegisterSystemRequest(); // RegisterSystemRequest |  (optional) 

            try
            {
                // Register or update a system with its events.
                Guid result = apiInstance.RegisterOrUpdateSystemAsync(registerSystemRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationsApi.RegisterOrUpdateSystemAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the RegisterOrUpdateSystemAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Register or update a system with its events.
    ApiResponse<Guid> response = apiInstance.RegisterOrUpdateSystemAsyncWithHttpInfo(registerSystemRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationsApi.RegisterOrUpdateSystemAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **registerSystemRequest** | [**RegisterSystemRequest**](RegisterSystemRequest.md) |  | [optional]  |

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
| **200** | Returns id of the newly registered or updated system |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="sendnotificationasync"></a>
# **SendNotificationAsync**
> bool SendNotificationAsync (SendNotificationRequestInput sendNotificationRequestInput = null)

Send notification to a user via users' selected channels or fallback to default(SMTP) channel.

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
    public class SendNotificationAsyncExample
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
            var apiInstance = new NotificationsApi(httpClient, config, httpClientHandler);
            var sendNotificationRequestInput = new SendNotificationRequestInput(); // SendNotificationRequestInput |  (optional) 

            try
            {
                // Send notification to a user via users' selected channels or fallback to default(SMTP) channel.
                bool result = apiInstance.SendNotificationAsync(sendNotificationRequestInput);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationsApi.SendNotificationAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SendNotificationAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Send notification to a user via users' selected channels or fallback to default(SMTP) channel.
    ApiResponse<bool> response = apiInstance.SendNotificationAsyncWithHttpInfo(sendNotificationRequestInput);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationsApi.SendNotificationAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sendNotificationRequestInput** | [**SendNotificationRequestInput**](SendNotificationRequestInput.md) |  | [optional]  |

### Return type

**bool**

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
| **202** | Returns true if the system managed to queue the notification for sending |  -  |
| **409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

