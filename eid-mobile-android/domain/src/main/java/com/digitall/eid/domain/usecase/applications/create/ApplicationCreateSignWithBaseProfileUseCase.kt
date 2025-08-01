package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.models.applications.create.ApplicationSignWithBaseProfileRequestModel
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class ApplicationCreateSignWithBaseProfileUseCase: BaseUseCase {

    companion object {
        private const val TAG = "ApplicationCreateSignWithBaseProfileUseCaseTag"
    }

    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    fun invoke(
        otpCode: String,
        firebaseId: String,
        mobileApplicationInstanceId: String,
    ) = applicationCreateNetworkRepository.signWithBaseProfile(
        data = ApplicationSignWithBaseProfileRequestModel(
            forceUpdate = true,
            otpCode = otpCode,
            firebaseId = firebaseId,
            mobileApplicationInstanceId = mobileApplicationInstanceId,
        ),
    )
}