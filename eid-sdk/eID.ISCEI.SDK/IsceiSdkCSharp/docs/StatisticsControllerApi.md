# IsceiSdk.Api.StatisticsControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReportDetailed**](StatisticsControllerApi.md#reportdetailed) | **GET** /api/v1/statistics/report/detailed |  |
| [**ReportRequestsCount**](StatisticsControllerApi.md#reportrequestscount) | **GET** /api/v1/statistics/report/requests-count |  |
| [**ReportRequestsTotal**](StatisticsControllerApi.md#reportrequeststotal) | **GET** /api/v1/statistics/report/requests-total |  |

<a id="reportdetailed"></a>
# **ReportDetailed**
> List&lt;List&lt;string&gt;&gt; ReportDetailed (DateTime createDateFrom, DateTime createDateTo, string systemType = null, string clientId = null, bool? isEmployee = null, bool? success = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using IsceiSdk.Api;
using IsceiSdk.Client;
using IsceiSdk.Model;

namespace Example
{
    public class ReportDetailedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://isceigwext.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: ISCEI
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new StatisticsControllerApi(httpClient, config, httpClientHandler);
            var createDateFrom = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime | 
            var createDateTo = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime | 
            var systemType = "systemType_example";  // string |  (optional) 
            var clientId = "clientId_example";  // string |  (optional) 
            var isEmployee = true;  // bool? |  (optional) 
            var success = true;  // bool? |  (optional) 

            try
            {
                List<List<string>> result = apiInstance.ReportDetailed(createDateFrom, createDateTo, systemType, clientId, isEmployee, success);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatisticsControllerApi.ReportDetailed: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReportDetailedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<List<string>>> response = apiInstance.ReportDetailedWithHttpInfo(createDateFrom, createDateTo, systemType, clientId, isEmployee, success);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatisticsControllerApi.ReportDetailedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createDateFrom** | **DateTime** |  |  |
| **createDateTo** | **DateTime** |  |  |
| **systemType** | **string** |  | [optional]  |
| **clientId** | **string** |  | [optional]  |
| **isEmployee** | **bool?** |  | [optional]  |
| **success** | **bool?** |  | [optional]  |

### Return type

**List<List<string>>**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="reportrequestscount"></a>
# **ReportRequestsCount**
> List&lt;List&lt;string&gt;&gt; ReportRequestsCount (DateTime createDateFrom, DateTime createDateTo, string systemType = null, string clientId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using IsceiSdk.Api;
using IsceiSdk.Client;
using IsceiSdk.Model;

namespace Example
{
    public class ReportRequestsCountExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://isceigwext.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: ISCEI
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new StatisticsControllerApi(httpClient, config, httpClientHandler);
            var createDateFrom = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime | 
            var createDateTo = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime | 
            var systemType = "systemType_example";  // string |  (optional) 
            var clientId = "clientId_example";  // string |  (optional) 

            try
            {
                List<List<string>> result = apiInstance.ReportRequestsCount(createDateFrom, createDateTo, systemType, clientId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatisticsControllerApi.ReportRequestsCount: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReportRequestsCountWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<List<string>>> response = apiInstance.ReportRequestsCountWithHttpInfo(createDateFrom, createDateTo, systemType, clientId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatisticsControllerApi.ReportRequestsCountWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **createDateFrom** | **DateTime** |  |  |
| **createDateTo** | **DateTime** |  |  |
| **systemType** | **string** |  | [optional]  |
| **clientId** | **string** |  | [optional]  |

### Return type

**List<List<string>>**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="reportrequeststotal"></a>
# **ReportRequestsTotal**
> List&lt;List&lt;string&gt;&gt; ReportRequestsTotal (int year)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using IsceiSdk.Api;
using IsceiSdk.Client;
using IsceiSdk.Model;

namespace Example
{
    public class ReportRequestsTotalExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://isceigwext.sandbox.bgeid.bg";
            // Configure Bearer token for authorization: ISCEI
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new StatisticsControllerApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | 

            try
            {
                List<List<string>> result = apiInstance.ReportRequestsTotal(year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatisticsControllerApi.ReportRequestsTotal: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReportRequestsTotalWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<List<string>>> response = apiInstance.ReportRequestsTotalWithHttpInfo(year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatisticsControllerApi.ReportRequestsTotalWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **year** | **int** |  |  |

### Return type

**List<List<string>>**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

