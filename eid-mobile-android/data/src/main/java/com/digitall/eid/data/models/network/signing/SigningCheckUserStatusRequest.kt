package com.digitall.eid.data.models.network.signing

import com.google.gson.annotations.SerializedName

data class SigningCheckUserStatusRequest(
    @SerializedName("uid") val uid: String?,
)
