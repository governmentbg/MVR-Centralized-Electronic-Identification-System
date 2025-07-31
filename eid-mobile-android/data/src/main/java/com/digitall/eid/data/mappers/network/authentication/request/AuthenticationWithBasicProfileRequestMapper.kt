package com.digitall.eid.data.mappers.network.authentication.request

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.request.BasicProfileAuthenticationRequest
import com.digitall.eid.domain.models.authentication.request.BasicProfileAuthenticationRequestModel
import com.digitall.eid.data.BuildConfig.CLIENT_ID

class AuthenticationWithBasicProfileRequestMapper :
    BaseMapper<BasicProfileAuthenticationRequestModel, BasicProfileAuthenticationRequest>() {

    override fun map(from: BasicProfileAuthenticationRequestModel): BasicProfileAuthenticationRequest {
        return with(from) {
            BasicProfileAuthenticationRequest(
                clientId = CLIENT_ID,
                email = email,
                password = password
            )
        }
    }
}