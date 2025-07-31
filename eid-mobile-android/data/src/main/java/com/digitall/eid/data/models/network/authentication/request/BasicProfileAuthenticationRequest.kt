package com.digitall.eid.data.models.network.authentication.request

import com.google.gson.annotations.SerializedName

data class BasicProfileAuthenticationRequest(
    @SerializedName("client_id") val clientId: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("password") val password: String?
)
