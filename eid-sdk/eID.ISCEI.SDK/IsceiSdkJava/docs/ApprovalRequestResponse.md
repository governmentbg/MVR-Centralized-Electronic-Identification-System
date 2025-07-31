

# ApprovalRequestResponse


## Properties

| Name | Type | Description | Notes |
|------------ | ------------- | ------------- | -------------|
|**id** | **UUID** |  |  [optional] |
|**username** | **String** |  |  [optional] |
|**levelOfAssurance** | [**LevelOfAssuranceEnum**](#LevelOfAssuranceEnum) |  |  [optional] |
|**requestFrom** | [**RequestFromDto**](RequestFromDto.md) |  |  [optional] |
|**createDate** | **OffsetDateTime** |  |  [optional] |
|**maxTtl** | **Long** |  |  [optional] |
|**expiresIn** | **Long** |  |  [optional] |



## Enum: LevelOfAssuranceEnum

| Name | Value |
|---- | -----|
| LOW | &quot;LOW&quot; |
| SUBSTANTIAL | &quot;SUBSTANTIAL&quot; |
| HIGH | &quot;HIGH&quot; |



