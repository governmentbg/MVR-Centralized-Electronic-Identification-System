

# EmpowermentStatementResult


## Properties

| Name | Type | Description | Notes |
|------------ | ------------- | ------------- | -------------|
|**id** | **UUID** |  |  [optional] |
|**number** | **String** |  |  [optional] |
|**startDate** | **OffsetDateTime** |  |  [optional] |
|**expiryDate** | **OffsetDateTime** |  |  [optional] |
|**status** | **EmpowermentStatementStatus** |  |  [optional] |
|**uid** | **String** |  |  [optional] |
|**uidType** | **IdentifierType** |  |  [optional] |
|**name** | **String** |  |  [optional] |
|**onBehalfOf** | **OnBehalfOf** |  |  [optional] |
|**authorizerUids** | [**List&lt;UidResult&gt;**](UidResult.md) |  |  [optional] |
|**empoweredUids** | [**List&lt;UidResult&gt;**](UidResult.md) |  |  [optional] |
|**providerId** | **String** |  |  [optional] |
|**providerName** | **String** |  |  [optional] |
|**serviceId** | **Integer** |  |  [optional] |
|**serviceName** | **String** |  |  [optional] |
|**volumeOfRepresentation** | [**List&lt;VolumeOfRepresentationResult&gt;**](VolumeOfRepresentationResult.md) |  |  [optional] |
|**createdOn** | **OffsetDateTime** |  |  [optional] |
|**createdBy** | **String** |  |  [optional] |
|**issuerPosition** | **String** |  |  [optional] |
|**xmlRepresentation** | **String** |  |  [optional] |
|**denialReason** | **EmpowermentsDenialReason** |  |  [optional] |
|**empowermentWithdrawals** | [**List&lt;EmpowermentWithdrawResult&gt;**](EmpowermentWithdrawResult.md) |  |  [optional] |
|**empowermentDisagreements** | [**List&lt;EmpowermentDisagreementResult&gt;**](EmpowermentDisagreementResult.md) |  |  [optional] |
|**statusHistory** | [**List&lt;StatusHistoryResult&gt;**](StatusHistoryResult.md) |  |  [optional] |
|**calculatedStatusOn** | **CalculatedEmpowermentStatus** |  |  [optional] |



