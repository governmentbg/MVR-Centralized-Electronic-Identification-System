package com.digitall.eid.domain.models.challenge.request

data class SignedChallengeRequestModel(
    val signature: String?,
    val challenge: String?,
    val certificate: String?,
    val certificateChain: List<String>?,
)
