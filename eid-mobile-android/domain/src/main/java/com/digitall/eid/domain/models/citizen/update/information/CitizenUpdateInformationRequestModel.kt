package com.digitall.eid.domain.models.citizen.update.information

data class CitizenUpdateInformationRequestModel(
    val firstName: String?,
    val secondName: String?,
    val lastName: String?,
    val firstNameLatin: String?,
    val secondNameLatin: String?,
    val lastNameLatin: String?,
    val phoneNumber: String?,
    val twoFaEnabled: Boolean?,
)
