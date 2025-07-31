# DeauApi

All URIs are relative to *https://roapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**approveEmpowermentByDeauAsync**](DeauApi.md#approveEmpowermentByDeauAsync) | **POST** /api/v1/Deau/approve-empowerment | Approve Unconfirmed empowerment |
| [**denyEmpowermentByDeauAsync**](DeauApi.md#denyEmpowermentByDeauAsync) | **POST** /api/v1/Deau/deny-empowerment | Deny both Active and Unconfirmed empowerments |
| [**getEmpowermentsByDeauAsync**](DeauApi.md#getEmpowermentsByDeauAsync) | **POST** /api/v1/Deau/empowerments | This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data. |


<a id="approveEmpowermentByDeauAsync"></a>
# **approveEmpowermentByDeauAsync**
> UUID approveEmpowermentByDeauAsync(approveEmpowermentByDeauRequest)

Approve Unconfirmed empowerment

### Example
```java
// Import classes:
import eid.sdk.ro.ApiClient;
import eid.sdk.ro.ApiException;
import eid.sdk.ro.Configuration;
import eid.sdk.ro.auth.*;
import eid.sdk.ro.models.*;
import org.openapitools.client.api.DeauApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://roapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    DeauApi apiInstance = new DeauApi(defaultClient);
    ApproveEmpowermentByDeauRequest approveEmpowermentByDeauRequest = new ApproveEmpowermentByDeauRequest(); // ApproveEmpowermentByDeauRequest | 
    try {
      UUID result = apiInstance.approveEmpowermentByDeauAsync(approveEmpowermentByDeauRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling DeauApi#approveEmpowermentByDeauAsync");
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
| **approveEmpowermentByDeauRequest** | [**ApproveEmpowermentByDeauRequest**](ApproveEmpowermentByDeauRequest.md)|  | [optional] |

### Return type

[**UUID**](UUID.md)

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

<a id="denyEmpowermentByDeauAsync"></a>
# **denyEmpowermentByDeauAsync**
> UUID denyEmpowermentByDeauAsync(denyEmpowermentByDeauRequest)

Deny both Active and Unconfirmed empowerments

### Example
```java
// Import classes:
import eid.sdk.ro.ApiClient;
import eid.sdk.ro.ApiException;
import eid.sdk.ro.Configuration;
import eid.sdk.ro.auth.*;
import eid.sdk.ro.models.*;
import org.openapitools.client.api.DeauApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://roapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    DeauApi apiInstance = new DeauApi(defaultClient);
    DenyEmpowermentByDeauRequest denyEmpowermentByDeauRequest = new DenyEmpowermentByDeauRequest(); // DenyEmpowermentByDeauRequest | 
    try {
      UUID result = apiInstance.denyEmpowermentByDeauAsync(denyEmpowermentByDeauRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling DeauApi#denyEmpowermentByDeauAsync");
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
| **denyEmpowermentByDeauRequest** | [**DenyEmpowermentByDeauRequest**](DenyEmpowermentByDeauRequest.md)|  | [optional] |

### Return type

[**UUID**](UUID.md)

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

<a id="getEmpowermentsByDeauAsync"></a>
# **getEmpowermentsByDeauAsync**
> EmpowermentStatementResultIPaginatedData getEmpowermentsByDeauAsync(getEmpowermentsByDeauRequest)

This endpoint will validate Deau and search for a Empowerments based on filter.  It may return either a 200 OK or 202 Accepted response.    - 202 Accepted: Indicates that validation checks for legal representation are still in progress.     The response will contain an empty list. You should retry the request after a short interval.    - 200 OK: All checks have been successfully completed. The response will contain the list of empowerments.    Clients integrating with this endpoint must handle both 202 and 200 status codes appropriately.   If a 202 is received, implement retry logic (e.g., with a delay or exponential backoff)   until a 200 OK is returned with the final data.

### Example
```java
// Import classes:
import eid.sdk.ro.ApiClient;
import eid.sdk.ro.ApiException;
import eid.sdk.ro.Configuration;
import eid.sdk.ro.auth.*;
import eid.sdk.ro.models.*;
import org.openapitools.client.api.DeauApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://roapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    DeauApi apiInstance = new DeauApi(defaultClient);
    GetEmpowermentsByDeauRequest getEmpowermentsByDeauRequest = new GetEmpowermentsByDeauRequest(); // GetEmpowermentsByDeauRequest | 
    try {
      EmpowermentStatementResultIPaginatedData result = apiInstance.getEmpowermentsByDeauAsync(getEmpowermentsByDeauRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling DeauApi#getEmpowermentsByDeauAsync");
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
| **getEmpowermentsByDeauRequest** | [**GetEmpowermentsByDeauRequest**](GetEmpowermentsByDeauRequest.md)|  | [optional] |

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

