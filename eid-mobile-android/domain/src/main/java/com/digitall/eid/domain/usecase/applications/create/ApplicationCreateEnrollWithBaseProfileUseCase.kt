package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithBaseProfileRequestModel
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class ApplicationCreateEnrollWithBaseProfileUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationCreateEnrollWithBaseProfileUseCaseTag"
    }

    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    fun invoke(
        otpCode: String,
        certificateSigningRequest: String
    ) = applicationCreateNetworkRepository.enrollWithBaseProfile(
        data = ApplicationEnrollWithBaseProfileRequestModel(
            otpCode = otpCode,
            certificateSigningRequest = certificateSigningRequest,
        ),
    )
}