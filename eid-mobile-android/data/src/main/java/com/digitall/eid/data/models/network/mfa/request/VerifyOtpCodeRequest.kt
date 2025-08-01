package com.digitall.eid.data.models.network.mfa.request

import com.google.gson.annotations.SerializedName

data class VerifyOtpCodeRequest(
    @SerializedName("sessionId") val sessionId: String?,
    @SerializedName("otp") val otp: String?,
)
