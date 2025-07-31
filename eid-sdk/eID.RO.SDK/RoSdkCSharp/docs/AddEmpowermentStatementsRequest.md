# RoSdk.Model.AddEmpowermentStatementsRequest
Used for creating new empowerment statements

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**OnBehalfOf** | **OnBehalfOf** |  | [optional] 
**Name** | **string** | Name of legal entity. When OnBehalfOf.Individual this is taken from the token. | [optional] 
**Uid** | **string** | Uid of legal entity. When OnBehalfOf.Individual this is taken from the token. | [optional] 
**UidType** | **IdentifierType** |  | [optional] 
**EmpoweredUids** | [**List&lt;UserIdentifierData&gt;**](UserIdentifierData.md) | List of EGNs or LNCHs of empowered people | [optional] 
**TypeOfEmpowerment** | **TypeOfEmpowerment** |  | [optional] 
**ProviderId** | **string** | Representation of provider - extended reference | [optional] 
**ProviderName** | **string** | Provider name, collected and stored in the moment of execution | [optional] 
**ServiceId** | **int** | Numeric representation of service, depends on selected provider - extended reference | [optional] 
**ServiceName** | **string** | Service Name, collected and stored in the moment of execution | [optional] 
**IssuerPosition** | **string** | Name of the position the issuer has in the legal entity | [optional] 
**VolumeOfRepresentation** | [**List&lt;VolumeOfRepresentationRequest&gt;**](VolumeOfRepresentationRequest.md) | List of all selected actions, that can be performed over selected service | [optional] 
**StartDate** | **DateTime** | UTC. On this date, once verified and signed, the empowerment can be considered active.  If not provided, the empowerment will become immediately active after signing.  Default: DateTime.UtcNow | [optional] 
**ExpiryDate** | **DateTime?** | UTC. Empowerment statement will be active before this moment. Must be at least 1 hour after current time.  Endless empowerment if this date is null | [optional] 
**AuthorizerUids** | [**List&lt;AuthorizerIdentifierData&gt;**](AuthorizerIdentifierData.md) | List of EGNs or LNCHs of Authorizer people | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

