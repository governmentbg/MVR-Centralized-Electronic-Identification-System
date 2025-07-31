package com.digitall.eid.domain.models.citizen.forgotten.password

data class CitizenForgottenPasswordRequestModel(
    val forname: String?,
    val middlename: String?,
    val surname: String?,
    val email: String?
)
