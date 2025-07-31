package com.digitall.eid.domain.models.verify.login

import com.digitall.eid.domain.models.verify.login.response.VerifyLoginResponseModel

data class VerifyLoginFullModel(
    val information: VerifyLoginResponseModel? = null,
    val tabToOpen: Int? = null
) {
    val isValid: Boolean
        get() = information != null && tabToOpen != null
}
