package com.digitall.eid.data.repository.network.citizen.forgotten

import com.digitall.eid.data.mappers.network.citizen.forgotten.password.request.CitizenForgottenPasswordRequestMapper
import com.digitall.eid.data.network.citizen.forgotten.CitizenForgottenApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.forgotten.password.CitizenForgottenPasswordRequestModel
import com.digitall.eid.domain.repository.network.citizen.forgotten.CitizenForgottenNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class CitizenForgottenNetworkRepositoryImpl: CitizenForgottenNetworkRepository, BaseRepository() {

    companion object {
        private const val TAG = "CitizenRegistrationNetworkRepositoryTag"
    }

    private val citizenForgottenApi: CitizenForgottenApi by inject()
    private val forgottenPasswordRequestMapper: CitizenForgottenPasswordRequestMapper by inject()

    override fun forgottenPassword(data: CitizenForgottenPasswordRequestModel) = flow {
        logDebug("forgottenPassword", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenForgottenApi.forgottenPassword(
                requestBody = forgottenPasswordRequestMapper.map(data)
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("forgottenPassword onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError(
                "forgottenPassword onFailure",
                message,
                TAG
            )
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