# X509CertificateCodeFlowControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**codeFlowAuth**](X509CertificateCodeFlowControllerApi.md#codeFlowAuth) | **POST** /api/v1/code-flow/auth |  |
| [**codeFlowToken**](X509CertificateCodeFlowControllerApi.md#codeFlowToken) | **GET** /api/v1/code-flow/token |  |


<a id="codeFlowAuth"></a>
# **codeFlowAuth**
> String codeFlowAuth(clientId, responseType, state, redirectUri, codeChallenge, codeChallengeMethod, signedChallenge, scope)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.X509CertificateCodeFlowControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    X509CertificateCodeFlowControllerApi apiInstance = new X509CertificateCodeFlowControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    String responseType = "responseType_example"; // String | 
    String state = "state_example"; // String | 
    String redirectUri = "redirectUri_example"; // String | 
    String codeChallenge = "codeChallenge_example"; // String | 
    String codeChallengeMethod = "codeChallengeMethod_example"; // String | 
    SignedChallenge signedChallenge = new SignedChallenge(); // SignedChallenge | 
    Set<String> scope = Arrays.asList(); // Set<String> | 
    try {
      String result = apiInstance.codeFlowAuth(clientId, responseType, state, redirectUri, codeChallenge, codeChallengeMethod, signedChallenge, scope);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling X509CertificateCodeFlowControllerApi#codeFlowAuth");
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
| **responseType** | **String**|  | |
| **state** | **String**|  | |
| **redirectUri** | **String**|  | |
| **codeChallenge** | **String**|  | |
| **codeChallengeMethod** | **String**|  | |
| **signedChallenge** | [**SignedChallenge**](SignedChallenge.md)|  | |
| **scope** | [**Set&lt;String&gt;**](String.md)|  | [optional] |

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

<a id="codeFlowToken"></a>
# **codeFlowToken**
> String codeFlowToken(clientId, grantType, code, redirectUri, codeVerifier, refreshToken)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.X509CertificateCodeFlowControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    X509CertificateCodeFlowControllerApi apiInstance = new X509CertificateCodeFlowControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    String grantType = "AUTHORIZATION_CODE"; // String | 
    String code = "code_example"; // String | 
    String redirectUri = "redirectUri_example"; // String | 
    String codeVerifier = "codeVerifier_example"; // String | 
    String refreshToken = "refreshToken_example"; // String | 
    try {
      String result = apiInstance.codeFlowToken(clientId, grantType, code, redirectUri, codeVerifier, refreshToken);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling X509CertificateCodeFlowControllerApi#codeFlowToken");
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
| **grantType** | **String**|  | [enum: AUTHORIZATION_CODE, REFRESH_TOKEN] |
| **code** | **String**|  | [optional] |
| **redirectUri** | **String**|  | [optional] |
| **codeVerifier** | **String**|  | [optional] |
| **refreshToken** | **String**|  | [optional] |

### Return type

**String**

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

