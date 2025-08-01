package com.digitall.eid.data.models.network.verify.login.response

import com.google.gson.annotations.SerializedName

data class VerifyLoginResponse(
    @SerializedName("message") val message: String?,
    @SerializedName("statusCode") val statusCode: Int?,
)
