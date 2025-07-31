package com.digitall.eid.data.mappers.network.authentication.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.request.AuthenticationChallengeRequest
import com.digitall.eid.domain.models.authentication.request.AuthenticationChallengeRequestModel

class AuthenticationChallengeRequestMapper :
    BaseMapper<AuthenticationChallengeRequestModel, AuthenticationChallengeRequest>() {

    override fun map(from: AuthenticationChallengeRequestModel): AuthenticationChallengeRequest {
        return with(from) {
            AuthenticationChallengeRequest(
                requestForm = requestForm,
                levelOfAssurance = levelOfAssurance?.type
            )
        }
    }
}