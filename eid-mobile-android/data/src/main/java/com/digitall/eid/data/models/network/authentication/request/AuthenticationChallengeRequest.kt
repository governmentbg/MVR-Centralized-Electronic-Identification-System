package com.digitall.eid.data.models.network.authentication.request

import com.google.gson.annotations.SerializedName


data class AuthenticationChallengeRequest(
    @SerializedName("requestForm") val requestForm: String?,
    @SerializedName("levelOfAssurance") val levelOfAssurance: String?
)