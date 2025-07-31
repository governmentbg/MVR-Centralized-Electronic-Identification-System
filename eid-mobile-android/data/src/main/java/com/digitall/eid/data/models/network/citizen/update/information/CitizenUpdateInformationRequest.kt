package com.digitall.eid.data.models.network.citizen.update.information

import com.google.gson.annotations.SerializedName

data class CitizenUpdateInformationRequest(
    @SerializedName("firstName") val firstName: String?,
    @SerializedName("secondName") val secondName: String?,
    @SerializedName("lastName") val lastName: String?,
    @SerializedName("firstNameLatin") val firstNameLatin: String?,
    @SerializedName("secondNameLatin") val secondNameLatin: String?,
    @SerializedName("lastNameLatin") val lastNameLatin: String?,
    @SerializedName("phoneNumber") val phoneNumber: String?,
    @SerializedName("is2FaEnabled") val twoFaEnabled: Boolean?,
)
