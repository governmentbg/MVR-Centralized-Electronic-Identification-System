package com.digitall.eid.data.models.network.registration

import com.google.gson.annotations.SerializedName

data class RegisterNewUserRequest(
    @SerializedName("firstName") val forname: String?,
    @SerializedName("secondName") val middlename: String?,
    @SerializedName("lastName") val surname: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("phineNumber") val phoneNumber: String?,
    @SerializedName("baseProfilePassword") val password: String?
)
