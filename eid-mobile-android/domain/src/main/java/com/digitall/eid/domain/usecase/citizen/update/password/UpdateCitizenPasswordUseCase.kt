package com.digitall.eid.domain.usecase.citizen.update.password

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.update.password.CitizenUpdatePasswordRequestModel
import com.digitall.eid.domain.repository.network.citizen.update.CitizenUpdateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class UpdateCitizenPasswordUseCase: BaseUseCase {

    companion object {
        private const val TAG = "UpdateCitizenPasswordUseCaseTag"
    }

    private val citizenUpdateNetworkRepository: CitizenUpdateNetworkRepository by inject()


    fun invoke(oldPassword: String?, newPassword: String?, confirmedPassword: String?): Flow<ResultEmittedData<Unit>> {
        logDebug("updateCitizenPassword", TAG)
        return citizenUpdateNetworkRepository.updatePassword(
            data = CitizenUpdatePasswordRequestModel(
                oldPassword = oldPassword,
                newPassword = newPassword,
                confirmedPassword = confirmedPassword
            )
        )
    }
}