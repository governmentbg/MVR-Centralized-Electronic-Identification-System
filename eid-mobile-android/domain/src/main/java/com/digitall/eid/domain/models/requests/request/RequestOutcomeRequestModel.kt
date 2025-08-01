package com.digitall.eid.domain.models.requests.request

import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel

data class RequestOutcomeRequestModel(
    val signedChallenge: SignedChallengeRequestModel?,
    val approvalRequestStatus: String?
)
