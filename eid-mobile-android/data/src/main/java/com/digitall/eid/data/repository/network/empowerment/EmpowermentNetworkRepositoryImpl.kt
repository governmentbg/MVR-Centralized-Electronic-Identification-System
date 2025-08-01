/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.empowerment

import com.digitall.eid.data.mappers.network.empowerment.common.all.EmpowermentRequestMapper
import com.digitall.eid.data.mappers.network.empowerment.common.all.EmpowermentResponseMapper
import com.digitall.eid.data.mappers.network.empowerment.legal.EmpowermentLegalRequestMapper
import com.digitall.eid.data.network.empowerment.EmpowermentApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentRequestModel
import com.digitall.eid.domain.models.empowerment.legal.EmpowermentLegalRequestModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class EmpowermentNetworkRepositoryImpl :
    EmpowermentNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "EmpowermentNetworkRepositoryTag"
    }

    private val empowermentApi: EmpowermentApi by inject()
    private val empowermentRequestMapper: EmpowermentRequestMapper by inject()
    private val empowermentLegalRequestMapper: EmpowermentLegalRequestMapper by inject()
    private val empowermentResponseMapper: EmpowermentResponseMapper by inject()

    override fun getEmpowermentFromMe(
        data: EmpowermentRequestModel,
    ) = flow {
        logDebug("getEmpowermentFromMe data: $data", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentApi.getEmpowermentFromMe(
                request = empowermentRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentFromMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentFromMe onFailure", message, TAG)
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

    override fun getEmpowermentToMe(
        data: EmpowermentRequestModel,
    ) = flow {
        logDebug("getEmpowermentFromMe data: $data", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentApi.getEmpowermentToMe(
                number = data.number,
                status = data.status,
                sortBy = data.sortBy,
                pageSize = data.pageSize,
                pageIndex = data.pageIndex,
                authorizer = data.authorizer,
                validToDate = data.validToDate,
                serviceName = data.serviceName,
                providerName = data.providerName,
                sortDirection = data.sortDirection,
                onBehalfOf = data.onBehalfOf,
                eik = data.eik,
                showOnlyNoExpiryDate = data.showOnlyNoExpiryDate,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentFromMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentFromMe onFailure", message, TAG)
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

    override fun getEmpowermentLegal(data: EmpowermentLegalRequestModel) = flow {
        logDebug("getEmpowermentLegal data: $data", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentApi.getEmpowermentLegal(
                request = empowermentLegalRequestMapper.map(data),
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentLegal onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentLegal onFailure", message, TAG)
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