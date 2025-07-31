

# AddEmpowermentStatementsRequest

Used for creating new empowerment statements

## Properties

| Name | Type | Description | Notes |
|------------ | ------------- | ------------- | -------------|
|**onBehalfOf** | **OnBehalfOf** |  |  [optional] |
|**name** | **String** | Name of legal entity. When OnBehalfOf.Individual this is taken from the token. |  [optional] |
|**uid** | **String** | Uid of legal entity. When OnBehalfOf.Individual this is taken from the token. |  [optional] |
|**uidType** | **IdentifierType** |  |  [optional] |
|**empoweredUids** | [**List&lt;UserIdentifierData&gt;**](UserIdentifierData.md) | List of EGNs or LNCHs of empowered people |  [optional] |
|**typeOfEmpowerment** | **TypeOfEmpowerment** |  |  [optional] |
|**providerId** | **String** | Representation of provider - extended reference |  [optional] |
|**providerName** | **String** | Provider name, collected and stored in the moment of execution |  [optional] |
|**serviceId** | **Integer** | Numeric representation of service, depends on selected provider - extended reference |  [optional] |
|**serviceName** | **String** | Service Name, collected and stored in the moment of execution |  [optional] |
|**issuerPosition** | **String** | Name of the position the issuer has in the legal entity |  [optional] |
|**volumeOfRepresentation** | [**List&lt;VolumeOfRepresentationRequest&gt;**](VolumeOfRepresentationRequest.md) | List of all selected actions, that can be performed over selected service |  [optional] |
|**startDate** | **OffsetDateTime** | UTC. On this date, once verified and signed, the empowerment can be considered active.  If not provided, the empowerment will become immediately active after signing.  Default: DateTime.UtcNow |  [optional] |
|**expiryDate** | **OffsetDateTime** | UTC. Empowerment statement will be active before this moment. Must be at least 1 hour after current time.  Endless empowerment if this date is null |  [optional] |
|**authorizerUids** | [**List&lt;AuthorizerIdentifierData&gt;**](AuthorizerIdentifierData.md) | List of EGNs or LNCHs of Authorizer people |  [optional] |



