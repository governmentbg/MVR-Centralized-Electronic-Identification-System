# CommonLoginControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**associateEidWithCitizenProfile**](CommonLoginControllerApi.md#associateEidWithCitizenProfile) | **POST** /api/v1/auth/associate-profiles |  |
| [**basicLogin**](CommonLoginControllerApi.md#basicLogin) | **POST** /api/v1/auth/basic |  |
| [**generateAuthenticationChallenge**](CommonLoginControllerApi.md#generateAuthenticationChallenge) | **POST** /api/v1/auth/generate-authentication-challenge |  |
| [**verifyOtp**](CommonLoginControllerApi.md#verifyOtp) | **POST** /api/v1/auth/verify-otp |  |


<a id="associateEidWithCitizenProfile"></a>
# **associateEidWithCitizenProfile**
> associateEidWithCitizenProfile(clientId, signedChallenge)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.CommonLoginControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    CommonLoginControllerApi apiInstance = new CommonLoginControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    SignedChallenge signedChallenge = new SignedChallenge(); // SignedChallenge | 
    try {
      apiInstance.associateEidWithCitizenProfile(clientId, signedChallenge);
    } catch (ApiException e) {
      System.err.println("Exception when calling CommonLoginControllerApi#associateEidWithCitizenProfile");
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
| **clientId** | **String**|  | |
| **signedChallenge** | [**SignedChallenge**](SignedChallenge.md)|  | |

### Return type

null (empty response body)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

<a id="basicLogin"></a>
# **basicLogin**
> Object basicLogin(basicLoginRequestDto)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.CommonLoginControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    CommonLoginControllerApi apiInstance = new CommonLoginControllerApi(defaultClient);
    BasicLoginRequestDto basicLoginRequestDto = new BasicLoginRequestDto(); // BasicLoginRequestDto | 
    try {
      Object result = apiInstance.basicLogin(basicLoginRequestDto);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling CommonLoginControllerApi#basicLogin");
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
| **basicLoginRequestDto** | [**BasicLoginRequestDto**](BasicLoginRequestDto.md)|  | |

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

<a id="generateAuthenticationChallenge"></a>
# **generateAuthenticationChallenge**
> AuthenticationRequestChallengeResponse generateAuthenticationChallenge(x509CertAuthenticationRequestDto)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.CommonLoginControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    CommonLoginControllerApi apiInstance = new CommonLoginControllerApi(defaultClient);
    X509CertAuthenticationRequestDto x509CertAuthenticationRequestDto = new X509CertAuthenticationRequestDto(); // X509CertAuthenticationRequestDto | 
    try {
      AuthenticationRequestChallengeResponse result = apiInstance.generateAuthenticationChallenge(x509CertAuthenticationRequestDto);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling CommonLoginControllerApi#generateAuthenticationChallenge");
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
| **x509CertAuthenticationRequestDto** | [**X509CertAuthenticationRequestDto**](X509CertAuthenticationRequestDto.md)|  | |

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

<a id="verifyOtp"></a>
# **verifyOtp**
> String verifyOtp(verifyOtpDto)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.CommonLoginControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    CommonLoginControllerApi apiInstance = new CommonLoginControllerApi(defaultClient);
    VerifyOtpDto verifyOtpDto = new VerifyOtpDto(); // VerifyOtpDto | 
    try {
      String result = apiInstance.verifyOtp(verifyOtpDto);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling CommonLoginControllerApi#verifyOtp");
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
| **verifyOtpDto** | [**VerifyOtpDto**](VerifyOtpDto.md)|  | |

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

