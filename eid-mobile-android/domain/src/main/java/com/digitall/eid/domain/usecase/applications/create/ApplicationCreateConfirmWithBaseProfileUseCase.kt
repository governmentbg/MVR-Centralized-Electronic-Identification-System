/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithBaseProfileRequestModel
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class ApplicationCreateConfirmWithBaseProfileUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationConfirmWithBaseProfileUseCaseTag"
    }

    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    fun invoke(
        otpCode: String,
    ) = applicationCreateNetworkRepository.confirmWithBaseProfile(
        data = ApplicationConfirmWithBaseProfileRequestModel(
            otpCode = otpCode,
            status = "OK",
        ),
    )
}