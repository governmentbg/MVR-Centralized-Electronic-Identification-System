package com.digitall.eid.data.models.network.citizen.update.email

import com.google.gson.annotations.SerializedName

data class CitizenUpdateEmailRequest(
    @SerializedName("email") val email: String?
)
