# IsceiSdk.Api.CommonLoginControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AssociateEidWithCitizenProfile**](CommonLoginControllerApi.md#associateeidwithcitizenprofile) | **POST** /api/v1/auth/associate-profiles |  |
| [**BasicLogin**](CommonLoginControllerApi.md#basiclogin) | **POST** /api/v1/auth/basic |  |
| [**GenerateAuthenticationChallenge**](CommonLoginControllerApi.md#generateauthenticationchallenge) | **POST** /api/v1/auth/generate-authentication-challenge |  |
| [**VerifyOtp**](CommonLoginControllerApi.md#verifyotp) | **POST** /api/v1/auth/verify-otp |  |

<a id="associateeidwithcitizenprofile"></a>
# **AssociateEidWithCitizenProfile**
> void AssociateEidWithCitizenProfile (string clientId, SignedChallenge signedChallenge)



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
    public class AssociateEidWithCitizenProfileExample
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
            var apiInstance = new CommonLoginControllerApi(httpClient, config, httpClientHandler);
            var clientId = "clientId_example";  // string | 
            var signedChallenge = new SignedChallenge(); // SignedChallenge | 

            try
            {
                apiInstance.AssociateEidWithCitizenProfile(clientId, signedChallenge);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CommonLoginControllerApi.AssociateEidWithCitizenProfile: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AssociateEidWithCitizenProfileWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.AssociateEidWithCitizenProfileWithHttpInfo(clientId, signedChallenge);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CommonLoginControllerApi.AssociateEidWithCitizenProfileWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **clientId** | **string** |  |  |
| **signedChallenge** | [**SignedChallenge**](SignedChallenge.md) |  |  |

### Return type

void (empty response body)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="basiclogin"></a>
# **BasicLogin**
> Object BasicLogin (BasicLoginRequestDto basicLoginRequestDto)



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
    public class BasicLoginExample
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
            var apiInstance = new CommonLoginControllerApi(httpClient, config, httpClientHandler);
            var basicLoginRequestDto = new BasicLoginRequestDto(); // BasicLoginRequestDto | 

            try
            {
                Object result = apiInstance.BasicLogin(basicLoginRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CommonLoginControllerApi.BasicLogin: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BasicLoginWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Object> response = apiInstance.BasicLoginWithHttpInfo(basicLoginRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CommonLoginControllerApi.BasicLoginWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **basicLoginRequestDto** | [**BasicLoginRequestDto**](BasicLoginRequestDto.md) |  |  |

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

<a id="generateauthenticationchallenge"></a>
# **GenerateAuthenticationChallenge**
> AuthenticationRequestChallengeResponse GenerateAuthenticationChallenge (X509CertAuthenticationRequestDto x509CertAuthenticationRequestDto)



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
    public class GenerateAuthenticationChallengeExample
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
            var apiInstance = new CommonLoginControllerApi(httpClient, config, httpClientHandler);
            var x509CertAuthenticationRequestDto = new X509CertAuthenticationRequestDto(); // X509CertAuthenticationRequestDto | 

            try
            {
                AuthenticationRequestChallengeResponse result = apiInstance.GenerateAuthenticationChallenge(x509CertAuthenticationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CommonLoginControllerApi.GenerateAuthenticationChallenge: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GenerateAuthenticationChallengeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AuthenticationRequestChallengeResponse> response = apiInstance.GenerateAuthenticationChallengeWithHttpInfo(x509CertAuthenticationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CommonLoginControllerApi.GenerateAuthenticationChallengeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **x509CertAuthenticationRequestDto** | [**X509CertAuthenticationRequestDto**](X509CertAuthenticationRequestDto.md) |  |  |

### Return type

[**AuthenticationRequestChallengeResponse**](AuthenticationRequestChallengeResponse.md)

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

<a id="verifyotp"></a>
# **VerifyOtp**
> string VerifyOtp (VerifyOtpDto verifyOtpDto)



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
    public class VerifyOtpExample
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
            var apiInstance = new CommonLoginControllerApi(httpClient, config, httpClientHandler);
            var verifyOtpDto = new VerifyOtpDto(); // VerifyOtpDto | 

            try
            {
                string result = apiInstance.VerifyOtp(verifyOtpDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CommonLoginControllerApi.VerifyOtp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the VerifyOtpWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.VerifyOtpWithHttpInfo(verifyOtpDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CommonLoginControllerApi.VerifyOtpWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **verifyOtpDto** | [**VerifyOtpDto**](VerifyOtpDto.md) |  |  |

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

