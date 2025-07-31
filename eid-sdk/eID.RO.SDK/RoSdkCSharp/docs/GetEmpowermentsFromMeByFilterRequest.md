# RoSdk.Model.GetEmpowermentsFromMeByFilterRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Number** | **string** | Empowerment number. Template: РОx/dd.mm.yyyy. x is a integer, dd.mm.yyyy the date of action. | [optional] 
**Status** | **EmpowermentsFromMeFilterStatus** |  | [optional] 
**Authorizer** | **string** | Empowerment Authorizer name - contains | [optional] 
**ProviderName** | **string** | Empowerment provider name | [optional] 
**ServiceName** | **string** | Empowerment Service name or Service code - contains | [optional] 
**ValidToDate** | **DateTime?** | Empowerment Valid to Date filter | [optional] 
**ShowOnlyNoExpiryDate** | **bool?** | Filter to show only never expiring empowerments | [optional] 
**EmpoweredUids** | [**List&lt;UidAndUidTypeData&gt;**](UidAndUidTypeData.md) | Filter to show only never expiring empowerments | [optional] 
**SortBy** | **EmpowermentsFromMeSortBy** |  | [optional] 
**SortDirection** | **SortDirection** |  | [optional] 
**OnBehalfOf** | **OnBehalfOf** |  | [optional] 
**PageSize** | **int** |  | [optional] 
**PageIndex** | **int** |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

