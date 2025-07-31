# IsceiSdk.Api.ApprovalRequestControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApprovalRequestAuth**](ApprovalRequestControllerApi.md#approvalrequestauth) | **POST** /api/v1/approval-request/auth/citizen |  |
| [**ApprovalRequestToken**](ApprovalRequestControllerApi.md#approvalrequesttoken) | **POST** /api/v1/approval-request/token |  |
| [**CibaRelyParty**](ApprovalRequestControllerApi.md#cibarelyparty) | **POST** /api/v1/approval-request/rely-party |  |
| [**EvaluateRequestOutcome**](ApprovalRequestControllerApi.md#evaluaterequestoutcome) | **POST** /api/v1/approval-request/outcome |  |
| [**GetUserApprovalRequests**](ApprovalRequestControllerApi.md#getuserapprovalrequests) | **GET** /api/v1/approval-request/user |  |

<a id="approvalrequestauth"></a>
# **ApprovalRequestAuth**
> Object ApprovalRequestAuth (string clientId, ApprovalAuthenticationRequestDto approvalAuthenticationRequestDto, List<string> scope = null)



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
    public class ApprovalRequestAuthExample
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
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var approvalAuthenticationRequestDto = new ApprovalAuthenticationRequestDto(); // ApprovalAuthenticationRequestDto | 
            var scope = new List<string>(); // List<string> |  (optional) 

            try
            {
                Object result = apiInstance.ApprovalRequestAuth(clientId, approvalAuthenticationRequestDto, scope);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.ApprovalRequestAuth: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApprovalRequestAuthWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Object> response = apiInstance.ApprovalRequestAuthWithHttpInfo(clientId, approvalAuthenticationRequestDto, scope);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApprovalRequestControllerApi.ApprovalRequestAuthWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **clientId** | **string** |  |  |
| **approvalAuthenticationRequestDto** | [**ApprovalAuthenticationRequestDto**](ApprovalAuthenticationRequestDto.md) |  |  |
| **scope** | [**List&lt;string&gt;**](string.md) |  | [optional]  |

### Return type

**Object**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="approvalrequesttoken"></a>
# **ApprovalRequestToken**
> string ApprovalRequestToken (string clientId, ApprovalRequestToken approvalRequestToken)



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
    public class ApprovalRequestTokenExample
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
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var approvalRequestToken = new ApprovalRequestToken(); // ApprovalRequestToken | 

            try
            {
                string result = apiInstance.ApprovalRequestToken(clientId, approvalRequestToken);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.ApprovalRequestToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApprovalRequestTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.ApprovalRequestTokenWithHttpInfo(clientId, approvalRequestToken);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApprovalRequestControllerApi.ApprovalRequestTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **clientId** | **string** |  |  |
| **approvalRequestToken** | [**ApprovalRequestToken**](ApprovalRequestToken.md) |  |  |

### Return type

**string**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="cibarelyparty"></a>
# **CibaRelyParty**
> DateTime CibaRelyParty (RelyPartyRequest relyPartyRequest)



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
    public class CibaRelyPartyExample
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
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);
            var relyPartyRequest = new RelyPartyRequest(); // RelyPartyRequest | 

            try
            {
                DateTime result = apiInstance.CibaRelyParty(relyPartyRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.CibaRelyParty: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CibaRelyPartyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<DateTime> response = apiInstance.CibaRelyPartyWithHttpInfo(relyPartyRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApprovalRequestControllerApi.CibaRelyPartyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **relyPartyRequest** | [**RelyPartyRequest**](RelyPartyRequest.md) |  |  |

### Return type

**DateTime**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="evaluaterequestoutcome"></a>
# **EvaluateRequestOutcome**
> string EvaluateRequestOutcome (Guid approvalRequestId, RequestOutcome requestOutcome)



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
    public class EvaluateRequestOutcomeExample
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
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);
            var approvalRequestId = "approvalRequestId_example";  // Guid | 
            var requestOutcome = new RequestOutcome(); // RequestOutcome | 

            try
            {
                string result = apiInstance.EvaluateRequestOutcome(approvalRequestId, requestOutcome);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.EvaluateRequestOutcome: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EvaluateRequestOutcomeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.EvaluateRequestOutcomeWithHttpInfo(approvalRequestId, requestOutcome);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApprovalRequestControllerApi.EvaluateRequestOutcomeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **approvalRequestId** | **Guid** |  |  |
| **requestOutcome** | [**RequestOutcome**](RequestOutcome.md) |  |  |

### Return type

**string**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getuserapprovalrequests"></a>
# **GetUserApprovalRequests**
> List&lt;ApprovalRequestResponse&gt; GetUserApprovalRequests ()



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
    public class GetUserApprovalRequestsExample
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
            var apiInstance = new ApprovalRequestControllerApi(httpClient, config, httpClientHandler);

            try
            {
                List<ApprovalRequestResponse> result = apiInstance.GetUserApprovalRequests();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApprovalRequestControllerApi.GetUserApprovalRequests: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetUserApprovalRequestsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ApprovalRequestResponse>> response = apiInstance.GetUserApprovalRequestsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApprovalRequestControllerApi.GetUserApprovalRequestsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;ApprovalRequestResponse&gt;**](ApprovalRequestResponse.md)

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

