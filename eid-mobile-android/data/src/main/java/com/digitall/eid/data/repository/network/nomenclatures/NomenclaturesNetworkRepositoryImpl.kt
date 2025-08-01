package com.digitall.eid.data.repository.network.nomenclatures

import com.digitall.eid.data.mappers.network.nomenclatures.reasons.NomenclaturesGetReasonsResponseMapper
import com.digitall.eid.data.network.nomenclatures.NomenclaturesApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class NomenclaturesNetworkRepositoryImpl :
    NomenclaturesNetworkRepository,
    BaseRepository() {

    companion object {
        const val TAG = "NomenclaturesNetworkRepositoryImplTag"
    }

    private val nomenclaturesApi: NomenclaturesApi by inject()
    private val nomenclaturesGetReasonsResponseMapper: NomenclaturesGetReasonsResponseMapper by inject()

    override fun getReasons() = flow {
        logDebug("getReasons", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            nomenclaturesApi.getReasons()
        }.onSuccess { model, message, responseCode ->
            logDebug("getReasons onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = nomenclaturesGetReasonsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError(
                "getReasons onFailure",
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