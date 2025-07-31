# ApprovalRequestControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**approvalRequestAuth**](ApprovalRequestControllerApi.md#approvalRequestAuth) | **POST** /api/v1/approval-request/auth/citizen |  |
| [**approvalRequestToken**](ApprovalRequestControllerApi.md#approvalRequestToken) | **POST** /api/v1/approval-request/token |  |
| [**cibaRelyParty**](ApprovalRequestControllerApi.md#cibaRelyParty) | **POST** /api/v1/approval-request/rely-party |  |
| [**evaluateRequestOutcome**](ApprovalRequestControllerApi.md#evaluateRequestOutcome) | **POST** /api/v1/approval-request/outcome |  |
| [**getUserApprovalRequests**](ApprovalRequestControllerApi.md#getUserApprovalRequests) | **GET** /api/v1/approval-request/user |  |


<a id="approvalRequestAuth"></a>
# **approvalRequestAuth**
> Object approvalRequestAuth(clientId, approvalAuthenticationRequestDto, scope)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    ApprovalAuthenticationRequestDto approvalAuthenticationRequestDto = new ApprovalAuthenticationRequestDto(); // ApprovalAuthenticationRequestDto | 
    Set<String> scope = Arrays.asList(); // Set<String> | 
    try {
      Object result = apiInstance.approvalRequestAuth(clientId, approvalAuthenticationRequestDto, scope);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#approvalRequestAuth");
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
| **approvalAuthenticationRequestDto** | [**ApprovalAuthenticationRequestDto**](ApprovalAuthenticationRequestDto.md)|  | |
| **scope** | [**Set&lt;String&gt;**](String.md)|  | [optional] |

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

<a id="approvalRequestToken"></a>
# **approvalRequestToken**
> String approvalRequestToken(clientId, approvalRequestToken)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    String clientId = "clientId_example"; // String | 
    ApprovalRequestToken approvalRequestToken = new ApprovalRequestToken(); // ApprovalRequestToken | 
    try {
      String result = apiInstance.approvalRequestToken(clientId, approvalRequestToken);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#approvalRequestToken");
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
| **approvalRequestToken** | [**ApprovalRequestToken**](ApprovalRequestToken.md)|  | |

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

<a id="cibaRelyParty"></a>
# **cibaRelyParty**
> OffsetDateTime cibaRelyParty(relyPartyRequest)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    RelyPartyRequest relyPartyRequest = new RelyPartyRequest(); // RelyPartyRequest | 
    try {
      OffsetDateTime result = apiInstance.cibaRelyParty(relyPartyRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#cibaRelyParty");
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
| **relyPartyRequest** | [**RelyPartyRequest**](RelyPartyRequest.md)|  | |

### Return type

[**OffsetDateTime**](OffsetDateTime.md)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: */*

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

<a id="evaluateRequestOutcome"></a>
# **evaluateRequestOutcome**
> String evaluateRequestOutcome(approvalRequestId, requestOutcome)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    UUID approvalRequestId = UUID.randomUUID(); // UUID | 
    RequestOutcome requestOutcome = new RequestOutcome(); // RequestOutcome | 
    try {
      String result = apiInstance.evaluateRequestOutcome(approvalRequestId, requestOutcome);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#evaluateRequestOutcome");
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
| **approvalRequestId** | **UUID**|  | |
| **requestOutcome** | [**RequestOutcome**](RequestOutcome.md)|  | |

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

<a id="getUserApprovalRequests"></a>
# **getUserApprovalRequests**
> List&lt;ApprovalRequestResponse&gt; getUserApprovalRequests()



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.ApprovalRequestControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    ApprovalRequestControllerApi apiInstance = new ApprovalRequestControllerApi(defaultClient);
    try {
      List<ApprovalRequestResponse> result = apiInstance.getUserApprovalRequests();
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling ApprovalRequestControllerApi#getUserApprovalRequests");
      System.err.println("Status code: " + e.getCode());
      System.err.println("Reason: " + e.getResponseBody());
      System.err.println("Response headers: " + e.getResponseHeaders());
      e.printStackTrace();
    }
  }
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

