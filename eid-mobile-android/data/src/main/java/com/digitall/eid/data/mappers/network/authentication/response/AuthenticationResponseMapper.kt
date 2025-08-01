/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.authentication.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.response.AuthenticationResponse
import com.digitall.eid.data.models.network.authentication.response.MFAResponse
import com.digitall.eid.data.models.network.authentication.response.TokenResponse
import com.digitall.eid.domain.models.authentication.response.AuthenticationResponseModel
import com.digitall.eid.domain.models.authentication.response.MFAResponseModel
import com.digitall.eid.domain.models.authentication.response.TokenResponseModel

class AuthenticationResponseMapper :
    BaseMapper<AuthenticationResponse, AuthenticationResponseModel>() {

    override fun map(from: AuthenticationResponse): AuthenticationResponseModel {
        return with(from) {
            AuthenticationResponseModel(
                data = when (data) {
                    is TokenResponse -> TokenResponseModel(
                        scope = data.scope,
                        expiresIn = data.expiresIn,
                        tokenType = data.tokenType,
                        accessToken = data.accessToken,
                        refreshToken = data.refreshToken,
                        sessionState = data.sessionState,
                        notBeforePolicy = data.notBeforePolicy,
                        refreshExpiresIn = data.refreshExpiresIn,
                    )

                    is MFAResponse -> MFAResponseModel(
                        mfaUrl = data.mfaUrl,
                        sessionId = data.sessionId,
                        ttl = data.ttl
                    )

                    else -> null
                }
            )
        }
    }

}