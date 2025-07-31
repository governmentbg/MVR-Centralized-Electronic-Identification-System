# # AddEmpowermentStatementsRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**on_behalf_of** | [**\RoSdk\Model\OnBehalfOf**](OnBehalfOf.md) |  | [optional]
**name** | **string** | Name of legal entity. When OnBehalfOf.Individual this is taken from the token. | [optional]
**uid** | **string** | Uid of legal entity. When OnBehalfOf.Individual this is taken from the token. | [optional]
**uid_type** | [**\RoSdk\Model\IdentifierType**](IdentifierType.md) |  | [optional]
**empowered_uids** | [**\RoSdk\Model\UserIdentifierData[]**](UserIdentifierData.md) | List of EGNs or LNCHs of empowered people | [optional]
**type_of_empowerment** | [**\RoSdk\Model\TypeOfEmpowerment**](TypeOfEmpowerment.md) |  | [optional]
**provider_id** | **string** | Representation of provider - extended reference | [optional]
**provider_name** | **string** | Provider name, collected and stored in the moment of execution | [optional]
**service_id** | **int** | Numeric representation of service, depends on selected provider - extended reference | [optional]
**service_name** | **string** | Service Name, collected and stored in the moment of execution | [optional]
**issuer_position** | **string** | Name of the position the issuer has in the legal entity | [optional]
**volume_of_representation** | [**\RoSdk\Model\VolumeOfRepresentationRequest[]**](VolumeOfRepresentationRequest.md) | List of all selected actions, that can be performed over selected service | [optional]
**start_date** | **\DateTime** | UTC. On this date, once verified and signed, the empowerment can be considered active.  If not provided, the empowerment will become immediately active after signing.  Default: DateTime.UtcNow | [optional]
**expiry_date** | **\DateTime** | UTC. Empowerment statement will be active before this moment. Must be at least 1 hour after current time.  Endless empowerment if this date is null | [optional]
**authorizer_uids** | [**\RoSdk\Model\AuthorizerIdentifierData[]**](AuthorizerIdentifierData.md) | List of EGNs or LNCHs of Authorizer people | [optional]

[[Back to Model list]](../../README.md#models) [[Back to API list]](../../README.md#endpoints) [[Back to README]](../../README.md)
