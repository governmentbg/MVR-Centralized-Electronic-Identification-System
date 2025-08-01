package com.digitall.eid.data.models.network.requests.request

import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import com.google.gson.annotations.SerializedName

data class RequestOutcomeRequest(
    @SerializedName("signedChallenge") val signedChallenge: SignedChallengeRequest?,
    @SerializedName("clientId") val clientId: String?,
    @SerializedName("approvalRequestStatus") val approvalRequestStatus: String?
)
