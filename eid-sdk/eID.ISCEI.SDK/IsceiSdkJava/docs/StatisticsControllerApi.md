# StatisticsControllerApi

All URIs are relative to *https://isceigwext.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**reportDetailed**](StatisticsControllerApi.md#reportDetailed) | **GET** /api/v1/statistics/report/detailed |  |
| [**reportRequestsCount**](StatisticsControllerApi.md#reportRequestsCount) | **GET** /api/v1/statistics/report/requests-count |  |
| [**reportRequestsTotal**](StatisticsControllerApi.md#reportRequestsTotal) | **GET** /api/v1/statistics/report/requests-total |  |


<a id="reportDetailed"></a>
# **reportDetailed**
> List&lt;List&lt;String&gt;&gt; reportDetailed(createDateFrom, createDateTo, systemType, clientId, isEmployee, success)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.StatisticsControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    StatisticsControllerApi apiInstance = new StatisticsControllerApi(defaultClient);
    OffsetDateTime createDateFrom = OffsetDateTime.now(); // OffsetDateTime | 
    OffsetDateTime createDateTo = OffsetDateTime.now(); // OffsetDateTime | 
    String systemType = "systemType_example"; // String | 
    String clientId = "clientId_example"; // String | 
    Boolean isEmployee = true; // Boolean | 
    Boolean success = true; // Boolean | 
    try {
      List<List<String>> result = apiInstance.reportDetailed(createDateFrom, createDateTo, systemType, clientId, isEmployee, success);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling StatisticsControllerApi#reportDetailed");
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
| **createDateFrom** | **OffsetDateTime**|  | |
| **createDateTo** | **OffsetDateTime**|  | |
| **systemType** | **String**|  | [optional] |
| **clientId** | **String**|  | [optional] |
| **isEmployee** | **Boolean**|  | [optional] |
| **success** | **Boolean**|  | [optional] |

### Return type

[**List&lt;List&lt;String&gt;&gt;**](List.md)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

<a id="reportRequestsCount"></a>
# **reportRequestsCount**
> List&lt;List&lt;String&gt;&gt; reportRequestsCount(createDateFrom, createDateTo, systemType, clientId)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.StatisticsControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    StatisticsControllerApi apiInstance = new StatisticsControllerApi(defaultClient);
    OffsetDateTime createDateFrom = OffsetDateTime.now(); // OffsetDateTime | 
    OffsetDateTime createDateTo = OffsetDateTime.now(); // OffsetDateTime | 
    String systemType = "systemType_example"; // String | 
    String clientId = "clientId_example"; // String | 
    try {
      List<List<String>> result = apiInstance.reportRequestsCount(createDateFrom, createDateTo, systemType, clientId);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling StatisticsControllerApi#reportRequestsCount");
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
| **createDateFrom** | **OffsetDateTime**|  | |
| **createDateTo** | **OffsetDateTime**|  | |
| **systemType** | **String**|  | [optional] |
| **clientId** | **String**|  | [optional] |

### Return type

[**List&lt;List&lt;String&gt;&gt;**](List.md)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

<a id="reportRequestsTotal"></a>
# **reportRequestsTotal**
> List&lt;List&lt;String&gt;&gt; reportRequestsTotal(year)



### Example
```java
// Import classes:
import eid.sdk.iscei.ApiClient;
import eid.sdk.iscei.ApiException;
import eid.sdk.iscei.Configuration;
import eid.sdk.iscei.auth.*;
import eid.sdk.iscei.models.*;
import org.openapitools.client.api.StatisticsControllerApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://isceigwext.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: ISCEI
    HttpBearerAuth ISCEI = (HttpBearerAuth) defaultClient.getAuthentication("ISCEI");
    ISCEI.setBearerToken("BEARER TOKEN");

    StatisticsControllerApi apiInstance = new StatisticsControllerApi(defaultClient);
    Integer year = 56; // Integer | 
    try {
      List<List<String>> result = apiInstance.reportRequestsTotal(year);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling StatisticsControllerApi#reportRequestsTotal");
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
| **year** | **Integer**|  | |

### Return type

[**List&lt;List&lt;String&gt;&gt;**](List.md)

### Authorization

[ISCEI](../README.md#ISCEI)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

