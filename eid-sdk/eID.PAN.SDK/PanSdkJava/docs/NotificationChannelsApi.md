# NotificationChannelsApi

All URIs are relative to *https://panapipublic.sandbox.bgeid.bg*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiV1NotificationChannelsGet**](NotificationChannelsApi.md#apiV1NotificationChannelsGet) | **GET** /api/v1/NotificationChannels | Get all notification channels |
| [**apiV1NotificationChannelsIdPut**](NotificationChannelsApi.md#apiV1NotificationChannelsIdPut) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels. |
| [**apiV1NotificationChannelsSelectedGet**](NotificationChannelsApi.md#apiV1NotificationChannelsSelectedGet) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels |
| [**apiV1NotificationChannelsSelectionPost**](NotificationChannelsApi.md#apiV1NotificationChannelsSelectionPost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels |
| [**registerAsync**](NotificationChannelsApi.md#registerAsync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels. |


<a id="apiV1NotificationChannelsGet"></a>
# **apiV1NotificationChannelsGet**
> UserNotificationChannelResultIPaginatedData apiV1NotificationChannelsGet(pageSize, pageIndex, channelName)

Get all notification channels

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    Integer pageSize = 100; // Integer | 
    Integer pageIndex = 1; // Integer | 
    String channelName = "channelName_example"; // String | 
    try {
      UserNotificationChannelResultIPaginatedData result = apiInstance.apiV1NotificationChannelsGet(pageSize, pageIndex, channelName);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#apiV1NotificationChannelsGet");
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
| **pageSize** | **Integer**|  | [optional] [default to 100] |
| **pageIndex** | **Integer**|  | [optional] [default to 1] |
| **channelName** | **String**|  | [optional] |

### Return type

[**UserNotificationChannelResultIPaginatedData**](UserNotificationChannelResultIPaginatedData.md)

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

<a id="apiV1NotificationChannelsIdPut"></a>
# **apiV1NotificationChannelsIdPut**
> apiV1NotificationChannelsIdPut(id, notificationChannelPayload)

Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    UUID id = UUID.randomUUID(); // UUID | 
    NotificationChannelPayload notificationChannelPayload = new NotificationChannelPayload(); // NotificationChannelPayload | 
    try {
      apiInstance.apiV1NotificationChannelsIdPut(id, notificationChannelPayload);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#apiV1NotificationChannelsIdPut");
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
| **id** | **UUID**|  | |
| **notificationChannelPayload** | [**NotificationChannelPayload**](NotificationChannelPayload.md)|  | [optional] |

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
| **200** | Success |  -  |
| **404** | Not Found |  -  |
| **409** | Conflict |  -  |

<a id="apiV1NotificationChannelsSelectedGet"></a>
# **apiV1NotificationChannelsSelectedGet**
> GuidIPaginatedData apiV1NotificationChannelsSelectedGet(pageSize, pageIndex)

Get user selected notification channels

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    Integer pageSize = 1000; // Integer | 
    Integer pageIndex = 1; // Integer | 
    try {
      GuidIPaginatedData result = apiInstance.apiV1NotificationChannelsSelectedGet(pageSize, pageIndex);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#apiV1NotificationChannelsSelectedGet");
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

<a id="apiV1NotificationChannelsSelectionPost"></a>
# **apiV1NotificationChannelsSelectionPost**
> apiV1NotificationChannelsSelectionPost(UUID)

Register selection of user notification channels

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    Set<UUID> UUID = Arrays.asList(); // Set<UUID> | 
    try {
      apiInstance.apiV1NotificationChannelsSelectionPost(UUID);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#apiV1NotificationChannelsSelectionPost");
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

<a id="registerAsync"></a>
# **registerAsync**
> UUID registerAsync(registerNotificationChannelRequest)

Add new notification channel in Pending table. Name must be unique against approved channels.

### Example
```java
// Import classes:
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;
import eid.sdk.pan.auth.*;
import eid.sdk.pan.models.*;
import org.openapitools.client.api.NotificationChannelsApi;

public class Example {
  public static void main(String[] args) {
    ApiClient defaultClient = Configuration.getDefaultApiClient();
    defaultClient.setBasePath("https://panapipublic.sandbox.bgeid.bg");
    
    // Configure HTTP bearer authorization: Bearer
    HttpBearerAuth Bearer = (HttpBearerAuth) defaultClient.getAuthentication("Bearer");
    Bearer.setBearerToken("BEARER TOKEN");

    NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);
    RegisterNotificationChannelRequest registerNotificationChannelRequest = new RegisterNotificationChannelRequest(); // RegisterNotificationChannelRequest | 
    try {
      UUID result = apiInstance.registerAsync(registerNotificationChannelRequest);
      System.out.println(result);
    } catch (ApiException e) {
      System.err.println("Exception when calling NotificationChannelsApi#registerAsync");
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
| **registerNotificationChannelRequest** | [**RegisterNotificationChannelRequest**](RegisterNotificationChannelRequest.md)|  | [optional] |

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
| **200** | Returns the Id of the created notification channel. |  -  |
| **409** | Conflict |  -  |

