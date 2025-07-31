# MobileControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**mobileX509CertificateLogin**](MobileControllerApi.md#mobileX509CertificateLogin) | **POST** /api/v1/auth/mobile/certificate-login |  |


<a id="mobileX509CertificateLogin"></a>
# **mobileX509CertificateLogin**
> String mobileX509CertificateLogin(mobileSignedChallenge)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.MobileControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    MobileControllerApi apiInstance = new MobileControllerApi(defaultClient);
    MobileSignedChallenge mobileSignedChallenge = new MobileSignedChallenge(); // MobileSignedChallenge | 
    try {
      String result = apiInstance.mobileX509CertificateLogin(mobileSignedChallenge);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling MobileControllerApi#mobileX509CertificateLogin");
      System.err.println("Status code: " + e.getCode());
      System.err.println("Reason: " + e.getResponseBody());
      System.err.println("Response headers: " + e.getResponseHeaders());
      e.printStackTrace();
    }
  }
}
```

### Parameters

| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **mobileSignedChallenge** | [**MobileSignedChallenge**](MobileSignedChallenge.md)|  | |

### Return type

**String**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

