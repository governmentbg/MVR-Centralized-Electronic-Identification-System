/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.empowerment

import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentCreateRequestMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentProviderResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentServiceScopeGetResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.create.EmpowermentServicesResponseMapper
import com.digitall.eid.data.network.empowerment.EmpowermentCreateApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.create.EmpowermentCreateModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCreateNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class EmpowermentCreateNetworkRepositoryImpl :
    EmpowermentCreateNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "EmpowermentCreateNetworkRepositoryTag"
    }

    private val empowermentCreateApi: EmpowermentCreateApi by inject()
    private val empowermentCreateRequestMapper: EmpowermentCreateRequestMapper by inject()
    private val empowermentProviderResponseMapper: EmpowermentProviderResponseMapper by inject()
    private val empowermentServicesResponseMapper: EmpowermentServicesResponseMapper by inject()
    private val empowermentServiceScopeGetResponseMapper: EmpowermentServiceScopeGetResponseMapper by inject()

    override fun getEmpowermentProviders(
        pageSize: Int,
        pageIndex: Int,
    ) = flow {
        logDebug("getEmpowermentProviders", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCreateApi.getProviders(
                name = null,
                pageSize = pageSize,
                pageIndex = pageIndex,
                includeDeleted = false,
                status = "Active", // or None
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentProviders onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentProviderResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentProviders onFailure", message, TAG)
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

    override fun createEmpowerment(
        data: EmpowermentCreateModel,
    ) = flow {
        logDebug("getServices", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCreateApi.createEmpowerment(
                request = empowermentCreateRequestMapper.map(data),
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("getServices onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getServices onFailure", message, TAG)
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

    override fun getEmpowermentServices(
        pageSize: Int,
        pageIndex: Int,
        providerId: String,
    ) = flow {
        logDebug("getServices", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCreateApi.getServices(
                pageSize = pageSize,
                pageIndex = pageIndex,
                includeEmpowermentOnly = true,
                providerId = providerId,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getServices onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentServicesResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getServices onFailure", message, TAG)
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

    override fun getEmpowermentServiceScope(
        serviceId: String,
    ) = flow {
        logDebug("getEmpowermentServiceScope serviceId: $serviceId", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCreateApi.getEmpowermentServiceScope(
                serviceId = serviceId,
                includeDeleted = false,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentServiceScope onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentServiceScopeGetResponseMapper.mapList(model),
                    message = message,
                    responseCode = responseCode
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentServiceScope onFailure", message, TAG)
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