package com.digitall.eid.domain.models.authentication.request

data class BasicProfileAuthenticationRequestModel(
    val email: String?,
    val password: String?,
)
