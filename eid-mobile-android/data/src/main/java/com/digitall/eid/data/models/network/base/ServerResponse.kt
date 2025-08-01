package com.digitall.eid.data.models.network.base

import com.google.gson.annotations.SerializedName

data class ServerResponse(
    @SerializedName("message") val message: String?,
    @SerializedName("statusCode") val statusCode: Int?,
)