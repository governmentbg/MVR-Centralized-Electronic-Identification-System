package com.digitall.eid.domain.usecase.citizen.update.email

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.update.email.CitizenUpdateEmailRequestModel
import com.digitall.eid.domain.repository.network.citizen.update.CitizenUpdateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class UpdateCitizenEmailUseCase : BaseUseCase {

    companion object {
        private const val TAG = "UpdateCitizenEmailUseCaseTag"
    }

    private val citizenUpdateNetworkRepository: CitizenUpdateNetworkRepository by inject()


    fun invoke(email: String?): Flow<ResultEmittedData<Unit>> {
        logDebug("updateCitizenEmail", TAG)
        return citizenUpdateNetworkRepository.updateEmail(
            data = CitizenUpdateEmailRequestModel(
                email = email
            )
        )
    }


}