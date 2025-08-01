package com.digitall.eid.data.models.network.authentication.response

import com.google.gson.annotations.SerializedName

data class AuthenticationChallengeResponse(
    @SerializedName("challenge") val challenge: String?,
)