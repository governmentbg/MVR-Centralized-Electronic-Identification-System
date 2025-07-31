package com.digitall.eid.domain.models.user

data class RegisterNewUserRequestModel(
    val forname: String?,
    val middlename: String?,
    val surname: String?,
    val email: String?,
    val phoneNumber: String?,
    val password: String?,
    val matchingPassword: String?,
)
