package com.digitall.eid.domain.models.mfa.request

data class VerifyOtpCodeRequestModel(
    val sessionId: String?,
    val otp: String?,
)
