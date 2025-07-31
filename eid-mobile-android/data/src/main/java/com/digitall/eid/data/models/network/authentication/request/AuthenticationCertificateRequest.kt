package com.digitall.eid.data.models.network.authentication.request

import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import com.google.gson.annotations.SerializedName

data class AuthenticationCertificateRequest(
    @SerializedName("signedChallenge") val signedChallenge: SignedChallengeRequest,
    @SerializedName("client_id") val clientId: String?,
)
