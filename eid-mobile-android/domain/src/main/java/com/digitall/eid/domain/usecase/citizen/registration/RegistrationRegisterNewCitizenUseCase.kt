package com.digitall.eid.domain.usecase.citizen.registration

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.register.CitizenRegisterNewUserRequestModel
import com.digitall.eid.domain.repository.network.citizen.registration.CitizenRegistrationNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class RegistrationRegisterNewCitizenUseCase : BaseUseCase {

    companion object {
        private const val TAG = "RegistrationRegisterNewCitizenUseCaseTag"
    }

    private val citizenRegistrationNetworkRepository: CitizenRegistrationNetworkRepository by inject()

    fun invoke(
        data: CitizenRegisterNewUserRequestModel
    ): Flow<ResultEmittedData<Unit>> {
        logDebug("registerNewUser", TAG)
        return citizenRegistrationNetworkRepository.registerNewCitizen(
            data = data
        )
    }
}