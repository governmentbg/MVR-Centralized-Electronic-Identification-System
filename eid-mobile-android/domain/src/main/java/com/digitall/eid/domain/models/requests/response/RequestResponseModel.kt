package com.digitall.eid.domain.models.requests.response

data class RequestResponseModel(
    val id: String?,
    val username: String?,
    val levelOfAssurance: String?,
    val requestFrom: RequestFromModel?,
    val createDate: String?,
    val maxTtl: Int?,
    val expiresIn: Long?,
)

data class RequestFromModel(
    val type: String?,
    val system: Map<String, String>?
)
