# IsceiSdk.Api.MobileControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**MobileX509CertificateLogin**](MobileControllerApi.md#mobilex509certificatelogin) | **POST** /api/v1/auth/mobile/certificate-login |  |

<a id="mobilex509certificatelogin"></a>
# **MobileX509CertificateLogin**
> string MobileX509CertificateLogin (MobileSignedChallenge mobileSignedChallenge)



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
    public class MobileX509CertificateLoginExample
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
            var apiInstance = new MobileControllerApi(httpClient, config, httpClientHandler);
            var mobileSignedChallenge = new MobileSignedChallenge(); // MobileSignedChallenge | 

            try
            {
                string result = apiInstance.MobileX509CertificateLogin(mobileSignedChallenge);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MobileControllerApi.MobileX509CertificateLogin: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the MobileX509CertificateLoginWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.MobileX509CertificateLoginWithHttpInfo(mobileSignedChallenge);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MobileControllerApi.MobileX509CertificateLoginWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **mobileSignedChallenge** | [**MobileSignedChallenge**](MobileSignedChallenge.md) |  |  |

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

