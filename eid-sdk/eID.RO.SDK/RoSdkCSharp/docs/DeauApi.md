# RoSdk.Api.DeauApi

All URIs are relative to *https://roapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApproveEmpowermentByDeauAsync**](DeauApi.md#approveempowermentbydeauasync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment |
| [**DenyEmpowermentByDeauAsync**](DeauApi.md#denyempowermentbydeauasync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments |
| [**GetEmpowermentsByDeauAsync**](DeauApi.md#getempowermentsbydeauasync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data. |

<a id="approveempowermentbydeauasync"></a>
# **ApproveEmpowermentByDeauAsync**
> Guid ApproveEmpowermentByDeauAsync (ApproveEmpowermentByDeauRequest approveEmpowermentByDeauRequest = null)

Approve Unconfirmed empowerment

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using RoSdk.Api;
using RoSdk.Client;
using RoSdk.Model;

namespace Example
{
    public class ApproveEmpowermentByDeauAsyncExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://roapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new DeauApi(httpClient, config, httpClientHandler);
            var approveEmpowermentByDeauRequest = new ApproveEmpowermentByDeauRequest(); // ApproveEmpowermentByDeauRequest |  (optional) 

            try
            {
                // Approve Unconfirmed empowerment
                Guid result = apiInstance.ApproveEmpowermentByDeauAsync(approveEmpowermentByDeauRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DeauApi.ApproveEmpowermentByDeauAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApproveEmpowermentByDeauAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Approve Unconfirmed empowerment
    ApiResponse<Guid> response = apiInstance.ApproveEmpowermentByDeauAsyncWithHttpInfo(approveEmpowermentByDeauRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DeauApi.ApproveEmpowermentByDeauAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **approveEmpowermentByDeauRequest** | [**ApproveEmpowermentByDeauRequest**](ApproveEmpowermentByDeauRequest.md) |  | [optional]  |

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
| **200** | Success |  -  |
| **403** | Forbidden |  -  |
| **404** | Not Found |  -  |
| **409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="denyempowermentbydeauasync"></a>
# **DenyEmpowermentByDeauAsync**
> Guid DenyEmpowermentByDeauAsync (DenyEmpowermentByDeauRequest denyEmpowermentByDeauRequest = null)

Deny both Active and Unconfirmed empowerments

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using RoSdk.Api;
using RoSdk.Client;
using RoSdk.Model;

namespace Example
{
    public class DenyEmpowermentByDeauAsyncExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://roapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new DeauApi(httpClient, config, httpClientHandler);
            var denyEmpowermentByDeauRequest = new DenyEmpowermentByDeauRequest(); // DenyEmpowermentByDeauRequest |  (optional) 

            try
            {
                // Deny both Active and Unconfirmed empowerments
                Guid result = apiInstance.DenyEmpowermentByDeauAsync(denyEmpowermentByDeauRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DeauApi.DenyEmpowermentByDeauAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DenyEmpowermentByDeauAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Deny both Active and Unconfirmed empowerments
    ApiResponse<Guid> response = apiInstance.DenyEmpowermentByDeauAsyncWithHttpInfo(denyEmpowermentByDeauRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DeauApi.DenyEmpowermentByDeauAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **denyEmpowermentByDeauRequest** | [**DenyEmpowermentByDeauRequest**](DenyEmpowermentByDeauRequest.md) |  | [optional]  |

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
| **200** | Success |  -  |
| **403** | Forbidden |  -  |
| **404** | Not Found |  -  |
| **409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getempowermentsbydeauasync"></a>
# **GetEmpowermentsByDeauAsync**
> EmpowermentStatementResultIPaginatedData GetEmpowermentsByDeauAsync (GetEmpowermentsByDeauRequest getEmpowermentsByDeauRequest = null)

This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using RoSdk.Api;
using RoSdk.Client;
using RoSdk.Model;

namespace Example
{
    public class GetEmpowermentsByDeauAsyncExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://roapipublic.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: Bearer
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new DeauApi(httpClient, config, httpClientHandler);
            var getEmpowermentsByDeauRequest = new GetEmpowermentsByDeauRequest(); // GetEmpowermentsByDeauRequest |  (optional) 

            try
            {
                // This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.
                EmpowermentStatementResultIPaginatedData result = apiInstance.GetEmpowermentsByDeauAsync(getEmpowermentsByDeauRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DeauApi.GetEmpowermentsByDeauAsync: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEmpowermentsByDeauAsyncWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.
    ApiResponse<EmpowermentStatementResultIPaginatedData> response = apiInstance.GetEmpowermentsByDeauAsyncWithHttpInfo(getEmpowermentsByDeauRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DeauApi.GetEmpowermentsByDeauAsyncWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **getEmpowermentsByDeauRequest** | [**GetEmpowermentsByDeauRequest**](GetEmpowermentsByDeauRequest.md) |  | [optional]  |

### Return type

[**EmpowermentStatementResultIPaginatedData**](EmpowermentStatementResultIPaginatedData.md)

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
| **202** | Accepted |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

