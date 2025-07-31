package com.digitall.eid.domain.models.authentication.request

import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel

data class AuthenticationCertificateRequestModel(
    val signedChallenge: SignedChallengeRequestModel
)