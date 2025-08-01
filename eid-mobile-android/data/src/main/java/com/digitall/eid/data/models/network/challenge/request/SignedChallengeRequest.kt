package com.digitall.eid.data.models.network.challenge.request

import com.google.gson.annotations.SerializedName

data class SignedChallengeRequest(
    @SerializedName("signature") val signature: String?,
    @SerializedName("challenge") val challenge: String?,
    @SerializedName("certificate") val certificate: String?,
    @SerializedName("certificateChain") val certificateChain: List<String>?,
)
