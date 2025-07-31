# IsceiSdk.Api.X509CertificateCodeFlowControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CodeFlowAuth**](X509CertificateCodeFlowControllerApi.md#codeflowauth) | **POST** /api/v1/code-flow/auth |  |
| [**CodeFlowToken**](X509CertificateCodeFlowControllerApi.md#codeflowtoken) | **GET** /api/v1/code-flow/token |  |

<a id="codeflowauth"></a>
# **CodeFlowAuth**
> string CodeFlowAuth (string clientId, string responseType, string state, string redirectUri, string codeChallenge, string codeChallengeMethod, SignedChallenge signedChallenge, List<string> scope = null)



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
    public class CodeFlowAuthExample
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
            var apiInstance = new X509CertificateCodeFlowControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var responseType = "responseType_example";  // string | 
            var state = "state_example";  // string | 
            var redirectUri = "redirectUri_example";  // string | 
            var codeChallenge = "codeChallenge_example";  // string | 
            var codeChallengeMethod = "codeChallengeMethod_example";  // string | 
            var signedChallenge = new SignedChallenge(); // SignedChallenge | 
            var scope = new List<string>(); // List<string> |  (optional) 

            try
            {
                string result = apiInstance.CodeFlowAuth(clientId, responseType, state, redirectUri, codeChallenge, codeChallengeMethod, signedChallenge, scope);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling X509CertificateCodeFlowControllerApi.CodeFlowAuth: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CodeFlowAuthWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CodeFlowAuthWithHttpInfo(clientId, responseType, state, redirectUri, codeChallenge, codeChallengeMethod, signedChallenge, scope);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling X509CertificateCodeFlowControllerApi.CodeFlowAuthWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **clientId** | **string** |  |  |
| **responseType** | **string** |  |  |
| **state** | **string** |  |  |
| **redirectUri** | **string** |  |  |
| **codeChallenge** | **string** |  |  |
| **codeChallengeMethod** | **string** |  |  |
| **signedChallenge** | [**SignedChallenge**](SignedChallenge.md) |  |  |
| **scope** | [**List&lt;string&gt;**](string.md) |  | [optional]  |

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

<a id="codeflowtoken"></a>
# **CodeFlowToken**
> string CodeFlowToken (string clientId, string grantType, string code = null, string redirectUri = null, string codeVerifier = null, string refreshToken = null)



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
    public class CodeFlowTokenExample
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
            var apiInstance = new X509CertificateCodeFlowControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var grantType = "AUTHORIZATION_CODE";  // string | 
            var code = "code_example";  // string |  (optional) 
            var redirectUri = "redirectUri_example";  // string |  (optional) 
            var codeVerifier = "codeVerifier_example";  // string |  (optional) 
            var refreshToken = "refreshToken_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.CodeFlowToken(clientId, grantType, code, redirectUri, codeVerifier, refreshToken);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling X509CertificateCodeFlowControllerApi.CodeFlowToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CodeFlowTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CodeFlowTokenWithHttpInfo(clientId, grantType, code, redirectUri, codeVerifier, refreshToken);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling X509CertificateCodeFlowControllerApi.CodeFlowTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **clientId** | **string** |  |  |
| **grantType** | **string** |  |  |
| **code** | **string** |  | [optional]  |
| **redirectUri** | **string** |  | [optional]  |
| **codeVerifier** | **string** |  | [optional]  |
| **refreshToken** | **string** |  | [optional]  |

### Return type

**string**

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

