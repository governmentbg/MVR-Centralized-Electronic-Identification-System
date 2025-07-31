package com.digitall.eid.domain.usecase.requests

import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import com.digitall.eid.domain.models.requests.request.RequestOutcomeRequestModel
import com.digitall.eid.domain.repository.network.requests.RequestsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class SetOutcomeRequestUseCase : BaseUseCase {

    companion object {
        private const val TAG = "SetOutcomeRequestUseCaseTag"
    }

    private val requestsNetworkRepository: RequestsNetworkRepository by inject()

    fun invoke(
        requestId: String?,
        status: String?,
        signedChallengeModel: SignedChallengeRequestModel? = null
    ) = requestsNetworkRepository.setRequestOutcome(
        requestId = requestId,
        outcome = RequestOutcomeRequestModel(
            signedChallenge = signedChallengeModel,
            approvalRequestStatus = status
        )
    )
}