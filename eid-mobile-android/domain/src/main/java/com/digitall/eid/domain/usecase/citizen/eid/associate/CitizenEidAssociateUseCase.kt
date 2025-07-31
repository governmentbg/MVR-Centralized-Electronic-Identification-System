package com.digitall.eid.domain.usecase.citizen.eid.associate

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import com.digitall.eid.domain.repository.network.citizen.eid.associate.CitizenEidAssociateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class CitizenEidAssociateUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CitizenEidAssociateUseCaseTag"
    }

    private val citizenEidAssociateNetworkRepository: CitizenEidAssociateNetworkRepository by inject()

    fun invoke(
        signature: String?,
        challenge: String?,
        certificate: String?,
        certificateChain: List<String>?
    ): Flow<ResultEmittedData<Unit>> {
        logDebug(
            "authenticateWithCertificate signature: $signature challenge: $challenge certificate: $certificate",
            TAG
        )

        return citizenEidAssociateNetworkRepository.associateEid(
            data = SignedChallengeRequestModel(
                signature = signature,
                challenge = challenge,
                certificate = certificate,
                certificateChain = certificateChain
            )
        )
    }
}