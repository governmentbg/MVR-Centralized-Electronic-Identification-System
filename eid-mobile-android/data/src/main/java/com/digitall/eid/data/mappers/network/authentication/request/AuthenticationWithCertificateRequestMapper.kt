package com.digitall.eid.data.mappers.network.authentication.request

import com.digitall.eid.data.BuildConfig.CLIENT_ID
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.request.AuthenticationCertificateRequest
import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import com.digitall.eid.domain.models.authentication.request.AuthenticationCertificateRequestModel

class AuthenticationWithCertificateRequestMapper :
    BaseMapper<AuthenticationCertificateRequestModel, AuthenticationCertificateRequest>() {

    override fun map(from: AuthenticationCertificateRequestModel): AuthenticationCertificateRequest {
        return with(from) {
            AuthenticationCertificateRequest(
                signedChallenge = SignedChallengeRequest(
                    signature = signedChallenge.signature,
                    certificate = signedChallenge.certificate,
                    challenge = signedChallenge.challenge,
                    certificateChain = signedChallenge.certificateChain,
                ),
                clientId = CLIENT_ID
            )
        }
    }
}