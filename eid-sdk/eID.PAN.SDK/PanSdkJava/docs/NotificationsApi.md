# NotificationsApi

All URIs are relative to *https://panapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiV1NotificationsDeactivatePost**](NotificationsApi.md#apiV1NotificationsDeactivatePost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events |
| [**apiV1NotificationsDeactivatedGet**](NotificationsApi.md#apiV1NotificationsDeactivatedGet) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications |
| [**apiV1NotificationsGet**](NotificationsApi.md#apiV1NotificationsGet) | **GET** /api/v1/Notifications | Get all Systems and notifications |
| [**registerOrUpdateSystemAsync**](NotificationsApi.md#registerOrUpdateSystemAsync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events. |
| [**sendNotificationAsync**](NotificationsApi.md#sendNotificationAsync) | **POST** /api/v1/Notifications/send | Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel. |


<a id="apiV1NotificationsDeactivatePost"></a>
# **apiV1NotificationsDeactivatePost**
> apiV1NotificationsDeactivatePost(UUID)

Register deactivated user events

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationsApi apiInstance = new NotificationsApi(defaultClient);
    Set<UUID> UUID = Arrays.asList(); // Set<UUID> | 
    try {
      apiInstance.apiV1NotificationsDeactivatePost(UUID);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationsApi#apiV1NotificationsDeactivatePost");
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
| **UUID** | [**Set&lt;UUID&gt;**](UUID.md)|  | [optional] |

### Return type

null (empty response body)

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
| **204** | No Content |  -  |

<a id="apiV1NotificationsDeactivatedGet"></a>
# **apiV1NotificationsDeactivatedGet**
> GuidIPaginatedData apiV1NotificationsDeactivatedGet(pageSize, pageIndex)

Get deactivated user notifications

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationsApi apiInstance = new NotificationsApi(defaultClient);
    Integer pageSize = 1000; // Integer | 
    Integer pageIndex = 1; // Integer | 
    try {
      GuidIPaginatedData result = apiInstance.apiV1NotificationsDeactivatedGet(pageSize, pageIndex);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationsApi#apiV1NotificationsDeactivatedGet");
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
| **pageSize** | **Integer**|  | [optional] [default to 1000] |
| **pageIndex** | **Integer**|  | [optional] [default to 1] |

### Return type

[**GuidIPaginatedData**](GuidIPaginatedData.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Success |  -  |

<a id="apiV1NotificationsGet"></a>
# **apiV1NotificationsGet**
> RegisteredSystemResultIPaginatedData apiV1NotificationsGet(pageSize, pageIndex, systemName)

Get all Systems and notifications

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationsApi apiInstance = new NotificationsApi(defaultClient);
    Integer pageSize = 50; // Integer | 
    Integer pageIndex = 1; // Integer | 
    String systemName = "systemName_example"; // String | 
    try {
      RegisteredSystemResultIPaginatedData result = apiInstance.apiV1NotificationsGet(pageSize, pageIndex, systemName);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationsApi#apiV1NotificationsGet");
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
| **pageSize** | **Integer**|  | [optional] [default to 50] |
| **pageIndex** | **Integer**|  | [optional] [default to 1] |
| **systemName** | **String**|  | [optional] |

### Return type

[**RegisteredSystemResultIPaginatedData**](RegisteredSystemResultIPaginatedData.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **400** | Bad Request |  -  |
| **500** | Server Error |  -  |
| **504** | Server Error |  -  |
| **200** | Success |  -  |

<a id="registerOrUpdateSystemAsync"></a>
# **registerOrUpdateSystemAsync**
> UUID registerOrUpdateSystemAsync(registerSystemRequest)

Register or update a system with its events.

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationsApi apiInstance = new NotificationsApi(defaultClient);
    RegisterSystemRequest registerSystemRequest = new RegisterSystemRequest(); // RegisterSystemRequest | 
    try {
      UUID result = apiInstance.registerOrUpdateSystemAsync(registerSystemRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationsApi#registerOrUpdateSystemAsync");
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
| **registerSystemRequest** | [**RegisterSystemRequest**](RegisterSystemRequest.md)|  | [optional] |

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
| **200** | Returns id of the newly registered or updated system |  -  |

<a id="sendNotificationAsync"></a>
# **sendNotificationAsync**
> Boolean sendNotificationAsync(sendNotificationRequestInput)

Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel.

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationsApi apiInstance = new NotificationsApi(defaultClient);
    SendNotificationRequestInput sendNotificationRequestInput = new SendNotificationRequestInput(); // SendNotificationRequestInput | 
    try {
      Boolean result = apiInstance.sendNotificationAsync(sendNotificationRequestInput);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationsApi#sendNotificationAsync");
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
| **sendNotificationRequestInput** | [**SendNotificationRequestInput**](SendNotificationRequestInput.md)|  | [optional] |

### Return type

**Boolean**

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
| **202** | Returns true if the system managed to queue the notification for sending |  -  |
| **409** | Conflict |  -  |

