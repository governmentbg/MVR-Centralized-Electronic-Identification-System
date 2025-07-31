package com.digitall.eid.data.models.network.citizen.registration

import com.google.gson.annotations.SerializedName

data class CitizenRegisterNewUserRequest(
    @SerializedName("firstName") val forname: String?,
    @SerializedName("secondName") val middlename: String?,
    @SerializedName("lastName") val surname: String?,
    @SerializedName("firstNameLatin") val fornameLatin: String?,
    @SerializedName("secondNameLatin") val middlenameLatin: String?,
    @SerializedName("lastNameLatin") val surnameLatin: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("phoneNumber") val phoneNumber: String?,
    @SerializedName("baseProfilePassword") val password: String?,
    @SerializedName("matchingPassword") val matchingPassword: String?,
)
