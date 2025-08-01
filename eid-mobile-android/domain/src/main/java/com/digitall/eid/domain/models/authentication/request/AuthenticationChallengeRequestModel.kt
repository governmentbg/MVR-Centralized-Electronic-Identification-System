package com.digitall.eid.domain.models.authentication.request

import com.digitall.eid.domain.models.common.LevelOfAssurance

data class AuthenticationChallengeRequestModel (
    val requestForm: String?,
    val levelOfAssurance: LevelOfAssurance?
)