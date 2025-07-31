package com.digitall.eid.domain.models.citizen.update.password

data class CitizenUpdatePasswordRequestModel(
    val oldPassword: String?,
    val newPassword: String?,
    val confirmedPassword: String?,
)
