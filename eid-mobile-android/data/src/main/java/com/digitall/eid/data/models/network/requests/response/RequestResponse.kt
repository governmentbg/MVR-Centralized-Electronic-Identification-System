package com.digitall.eid.data.models.network.requests.response

import com.google.gson.annotations.SerializedName

data class RequestResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("username") val username: String?,
    @SerializedName("levelOfAssurance") val levelOfAssurance: String?,
    @SerializedName("requestFrom") val requestFrom: RequestFromResponse?,
    @SerializedName("createDate") val createDate: String?,
    @SerializedName("maxTtl") val maxTtl: Int?,
    @SerializedName("expiresIn") val expiresIn: Long?
)

data class RequestFromResponse(
    @SerializedName("type") val type: String?,
    @SerializedName("system") val system: Map<String, String>?
)
