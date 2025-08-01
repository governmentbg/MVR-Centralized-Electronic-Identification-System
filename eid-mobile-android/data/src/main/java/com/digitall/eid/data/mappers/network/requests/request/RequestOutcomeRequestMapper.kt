package com.digitall.eid.data.mappers.network.requests.request

import com.digitall.eid.data.BuildConfig.CLIENT_ID
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.challenge.request.SignedChallengeRequest
import com.digitall.eid.data.models.network.requests.request.RequestOutcomeRequest
import com.digitall.eid.domain.models.requests.request.RequestOutcomeRequestModel

class RequestOutcomeRequestMapper :
    BaseMapper<RequestOutcomeRequestModel, RequestOutcomeRequest>() {

    override fun map(from: RequestOutcomeRequestModel): RequestOutcomeRequest {
        return with(from) {
            RequestOutcomeRequest(
                signedChallenge = signedChallenge?.let {
                    SignedChallengeRequest(
                        challenge = it.challenge,
                        signature = it.signature,
                        certificate = it.certificate,
                        certificateChain = it.certificateChain
                    )
                },
                clientId = CLIENT_ID,
                approvalRequestStatus = approvalRequestStatus
            )
        }
    }

}