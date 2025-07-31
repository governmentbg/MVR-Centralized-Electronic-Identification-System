package com.digitall.eid.domain.models.citizen.register

data class CitizenRegisterNewUserRequestModel(
    val forname: String?,
    val middlename: String?,
    val surname: String?,
    val fornameLatin: String?,
    val middlenameLatin: String?,
    val surnameLatin: String?,
    val email: String?,
    val phoneNumber: String?,
    val password: String?,
    val matchingPassword: String?,
)
