package com.digitall.eid.data.repository.network.citizen.registration

import com.digitall.eid.data.mappers.network.citizen.registration.request.RegisterNewCitizenRequestMapper
import com.digitall.eid.data.network.citizen.registration.CitizenRegistrationApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.register.CitizenRegisterNewUserRequestModel
import com.digitall.eid.domain.repository.network.citizen.registration.CitizenRegistrationNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class CitizenRegistrationNetworkRepositoryImpl: CitizenRegistrationNetworkRepository, BaseRepository() {

    private val citizenRegistrationApi: CitizenRegistrationApi by inject()
    private val registerNewCitizenRequestMapper: RegisterNewCitizenRequestMapper by inject()

    companion object {
        private const val TAG = "CitizenRegistrationNetworkRepositoryTag"
    }

    override fun registerNewCitizen(data: CitizenRegisterNewUserRequestModel) = flow {
        logDebug("registerNewUser", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenRegistrationApi.registerNewUser(requestBody = registerNewCitizenRequestMapper.map(data))
        }.onSuccess { _, message, responseCode ->
            logDebug("registerNewUser onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("registerNewUser onFailure", message, TAG)
            emit(
                ResultEmittedData.error(
                    model = null,
                    error = error,
                    title = title,
                    message = message,
                    errorType = errorType,
                    responseCode = responseCode,
                )
            )
        }
    }.flowOn(Dispatchers.IO)


}