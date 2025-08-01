package com.digitall.eid.data.repository.network.devices

import com.digitall.eid.data.mappers.network.devices.response.DevicesResponseMapper
import com.digitall.eid.data.network.devices.DevicesApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.devices.DevicesNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class DevicesNetworkRepositoryImpl :
    DevicesNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "DevicesNetworkRepositoryTag"
    }

    private val devicesApi: DevicesApi by inject()

    private val devicesResponseMapper: DevicesResponseMapper by inject()

    override fun getDevices() = flow {
        logDebug("getDevices", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            devicesApi.getDevices()
        }.onSuccess { model, message, responseCode ->
            logDebug("getDevices onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = devicesResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getDevices onFailure", message, TAG)
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