package com.digitall.eid.domain.models.verify.login.request

data class VerifyLoginRequestModel(
    val mobileApplicationInstanceId: String?,
    val firebaseId: String?,
    val forceUpdate: Boolean? = true
)
