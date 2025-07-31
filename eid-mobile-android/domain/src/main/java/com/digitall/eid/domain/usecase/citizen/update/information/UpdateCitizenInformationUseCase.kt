package com.digitall.eid.domain.usecase.citizen.update.information

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import com.digitall.eid.domain.repository.network.citizen.update.CitizenUpdateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class UpdateCitizenInformationUseCase : BaseUseCase {

    companion object {
        private const val TAG = "UpdateCitizenPhoneUseCaseTag"
    }

    private val citizenUpdateNetworkRepository: CitizenUpdateNetworkRepository by inject()

    fun invoke(data: CitizenUpdateInformationRequestModel): Flow<ResultEmittedData<Unit>> {
        logDebug("updateCitizenPhone", TAG)
        return citizenUpdateNetworkRepository.updateInformation(
            data = data
        )
    }
}