package com.digitall.eid.data.repository.network.citizen.update

import com.digitall.eid.data.mappers.network.citizen.update.infromation.request.CitizenUpdateInformationRequestMapper
import com.digitall.eid.data.network.citizen.update.CitizenUpdateApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.citizen.update.email.CitizenUpdateEmailRequestModel
import com.digitall.eid.domain.models.citizen.update.password.CitizenUpdatePasswordRequestModel
import com.digitall.eid.domain.models.citizen.update.information.CitizenUpdateInformationRequestModel
import com.digitall.eid.domain.repository.network.citizen.update.CitizenUpdateNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class CitizenUpdateNetworkRepositoryImpl : CitizenUpdateNetworkRepository, BaseRepository() {

    companion object {
        private const val TAG = "CitizenRegistrationNetworkRepositoryTag"
    }

    private val citizenUpdateApi: CitizenUpdateApi by inject()

    private val citizenUpdateInformationRequestMapper: CitizenUpdateInformationRequestMapper by inject()

    override fun updateEmail(data: CitizenUpdateEmailRequestModel) = flow {
        logDebug("updateEmail", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenUpdateApi.updateEmail(email = data.email)
        }.onSuccess { _, message, responseCode ->
            logDebug("updateEmail onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("updateEmail onFailure", message, TAG)
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

    override fun updatePassword(data: CitizenUpdatePasswordRequestModel) = flow {
        logDebug("updatePassword", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenUpdateApi.updatePassword(
                oldPassword = data.oldPassword,
                newPassword = data.newPassword,
                confirmedPassword = data.confirmedPassword
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("updatePassword onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("updatePassword onFailure", message, TAG)
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

    override fun updateInformation(data: CitizenUpdateInformationRequestModel) = flow {
        logDebug("updatePassword", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            citizenUpdateApi.updateInformation(
                requestBody = citizenUpdateInformationRequestMapper.map(data)
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("updatePassword onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("updatePassword onFailure", message, TAG)
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