package com.digitall.eid.data.models.network.verify.login.request

import com.google.gson.annotations.SerializedName

data class VerifyLoginRequest(
    @SerializedName("mobileApplicationInstanceId") val mobileApplicationInstanceId: String?,
    @SerializedName("firebaseId") val firebaseId: String?,
    @SerializedName("forceUpdate") val forceUpdate: Boolean?,
)
