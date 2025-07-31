# # GetEmpowermentsFromMeByFilterRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**number** | **string** | Empowerment number. Template: РОx/dd.mm.yyyy. x is a integer, dd.mm.yyyy the date of action. | [optional]
**status** | [**\RoSdk\Model\EmpowermentsFromMeFilterStatus**](EmpowermentsFromMeFilterStatus.md) |  | [optional]
**authorizer** | **string** | Empowerment Authorizer name - contains | [optional]
**provider_name** | **string** | Empowerment provider name | [optional]
**service_name** | **string** | Empowerment Service name or Service code - contains | [optional]
**valid_to_date** | **\DateTime** | Empowerment Valid to Date filter | [optional]
**show_only_no_expiry_date** | **bool** | Filter to show only never expiring empowerments | [optional]
**empowered_uids** | [**\RoSdk\Model\UidAndUidTypeData[]**](UidAndUidTypeData.md) | Filter to show only never expiring empowerments | [optional]
**sort_by** | [**\RoSdk\Model\EmpowermentsFromMeSortBy**](EmpowermentsFromMeSortBy.md) |  | [optional]
**sort_direction** | [**\RoSdk\Model\SortDirection**](SortDirection.md) |  | [optional]
**on_behalf_of** | [**\RoSdk\Model\OnBehalfOf**](OnBehalfOf.md) |  | [optional]
**page_size** | **int** |  | [optional]
**page_index** | **int** |  | [optional]

[[Back to Model list]](../../README.md#models) [[Back to API list]](../../README.md#endpoints) [[Back to README]](../../README.md)
