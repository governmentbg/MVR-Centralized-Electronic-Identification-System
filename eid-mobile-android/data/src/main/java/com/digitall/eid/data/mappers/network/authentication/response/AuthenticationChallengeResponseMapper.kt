package com.digitall.eid.data.mappers.network.authentication.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.response.AuthenticationChallengeResponse
import com.digitall.eid.domain.models.authentication.response.AuthenticationChallengeResponseModel

class AuthenticationChallengeResponseMapper :
    BaseMapper<AuthenticationChallengeResponse, AuthenticationChallengeResponseModel>() {

    override fun map(from: AuthenticationChallengeResponse): AuthenticationChallengeResponseModel {
        return with(from) {
            AuthenticationChallengeResponseModel(challenge = challenge)
        }
    }
}