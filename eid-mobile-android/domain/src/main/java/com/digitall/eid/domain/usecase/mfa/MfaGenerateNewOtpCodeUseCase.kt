package com.digitall.eid.domain.usecase.mfa

import com.digitall.eid.domain.models.mfa.request.GenerateNewOtpCodeRequestModel
import com.digitall.eid.domain.repository.network.mfa.MfaNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class MfaGenerateNewOtpCodeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "MfaGenerateNewOtpCodeUseCaseTag"
    }

    private val mfaNetworkRepository: MfaNetworkRepository by inject()

    fun invoke(sessionId: String?) = mfaNetworkRepository.generateNewOtpCode(
        data = GenerateNewOtpCodeRequestModel(
            sessionId = sessionId,
        )
    )
}