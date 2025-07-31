package com.digitall.eid.domain.usecase.citizen.forgotten.password

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.citizen.forgotten.password.CitizenForgottenPasswordRequestModel
import com.digitall.eid.domain.repository.network.citizen.forgotten.CitizenForgottenNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class CitizenForgottenPasswordUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CitizenForgottenPasswordUseCaseTag"
    }

    private val citizenForgottenNetworkRepository: CitizenForgottenNetworkRepository by inject()

    fun invoke(
        forname: String?,
        middlename: String?,
        surname: String?,
        email: String?
    ): Flow<ResultEmittedData<Unit>> {
        logDebug("forgottenPassword", TAG)
        return citizenForgottenNetworkRepository.forgottenPassword(
            data = CitizenForgottenPasswordRequestModel(
                forname = forname,
                middlename = middlename,
                surname = surname,
                email = email
            )
        )
    }
}