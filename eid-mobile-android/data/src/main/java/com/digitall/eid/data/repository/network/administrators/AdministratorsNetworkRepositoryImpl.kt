package com.digitall.eid.data.repository.network.administrators

import com.digitall.eid.data.mappers.network.administrators.AdministratorFrontOfficesResponseMapper
import com.digitall.eid.data.mappers.network.administrators.AdministratorsResponseMapper
import com.digitall.eid.data.network.administrators.AdministratorsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.administrators.AdministratorsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class AdministratorsNetworkRepositoryImpl :
    AdministratorsNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "AdministratorsNetworkRepositoryImplTag"
    }

    private val administratorsApi: AdministratorsApi by inject()
    private val administratorFrontOfficesResponseMapper: AdministratorFrontOfficesResponseMapper by inject()
    private val administratorsResponseMapper: AdministratorsResponseMapper by inject()

    override fun getAdministrators() = flow {
        logDebug("getAdministrators", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            administratorsApi.getAdministrators()
        }.onSuccess { model, message, responseCode ->
            logDebug("getAdministrators onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = administratorsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getAdministrators onFailure", message, TAG)
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

    override fun getAdministratorFrontOffices(eidAdministratorId: String) = flow {
        logDebug("getAdministratorFrontOffices", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            administratorsApi.getAdministratorFromOffices(id = eidAdministratorId)
        }.onSuccess { model, message, responseCode ->
            logDebug("getAdministratorFrontOffices onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = administratorFrontOfficesResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getAdministratorFrontOffices onFailure", message, TAG)
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