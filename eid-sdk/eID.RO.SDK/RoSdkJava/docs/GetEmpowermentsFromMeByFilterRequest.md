

# GetEmpowermentsFromMeByFilterRequest


## Properties

| Name | Type | Description | Notes |
|------------ | ------------- | ------------- | -------------|
|**number** | **String** | Empowerment number. Template: РОx/dd.mm.yyyy. x is a integer, dd.mm.yyyy the date of action. |  [optional] |
|**status** | **EmpowermentsFromMeFilterStatus** |  |  [optional] |
|**authorizer** | **String** | Empowerment Authorizer name - contains |  [optional] |
|**providerName** | **String** | Empowerment provider name |  [optional] |
|**serviceName** | **String** | Empowerment Service name or Service code - contains |  [optional] |
|**validToDate** | **OffsetDateTime** | Empowerment Valid to Date filter |  [optional] |
|**showOnlyNoExpiryDate** | **Boolean** | Filter to show only never expiring empowerments |  [optional] |
|**empoweredUids** | [**List&lt;UidAndUidTypeData&gt;**](UidAndUidTypeData.md) | Filter to show only never expiring empowerments |  [optional] |
|**sortBy** | **EmpowermentsFromMeSortBy** |  |  [optional] |
|**sortDirection** | **SortDirection** |  |  [optional] |
|**onBehalfOf** | **OnBehalfOf** |  |  [optional] |
|**pageSize** | **Integer** |  |  [optional] |
|**pageIndex** | **Integer** |  |  [optional] |



