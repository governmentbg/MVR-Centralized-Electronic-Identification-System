package com.digitall.eid.data.models.network.citizen.forgotten.password

import com.google.gson.annotations.SerializedName

data class CitizenForgottenPasswordRequest(
    @SerializedName("firstName") val forname: String?,
    @SerializedName("middleName") val middlename: String?,
    @SerializedName("lastName") val surname: String?,
    @SerializedName("email") val email: String?
)
