/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.empowerment

import com.digitall.eid.data.mappers.network.empowerment.signing.EmpowermentReasonResponseMapper
import com.digitall.eid.data.models.network.empowerment.cancel.EmpowermentCancelRequestModel
import com.digitall.eid.data.network.empowerment.EmpowermentCancelApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentCancelNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class EmpowermentCancelNetworkRepositoryImpl :
    EmpowermentCancelNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "EmpowermentCancelNetworkRepositoryTag"
    }

    private val empowermentCancelApi: EmpowermentCancelApi by inject()
    private val empowermentReasonResponseMapper: EmpowermentReasonResponseMapper by inject()

    override fun getEmpowermentFromMeCancelReasons() = flow {
        logDebug("getEmpowermentWithdrawReasons", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCancelApi.getEmpowermentFromMeCancelReasons()
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentWithdrawReasons onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentReasonResponseMapper.map(model.first()),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentWithdrawReasons onFailure", message, TAG)
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

    override fun getEmpowermentToMeCancelReasons() = flow {
        logDebug("getEmpowermentDisagreementReasons", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCancelApi.getEmpowermentToMeCancelReasons()
        }.onSuccess { model, message, responseCode ->
            logDebug("getEmpowermentDisagreementReasons onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = empowermentReasonResponseMapper.map(model.first()),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getEmpowermentDisagreementReasons onFailure", message, TAG)
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

    override fun cancelEmpowermentFromMe(
        reason: String,
        empowermentId: String
    ) = flow {
        logDebug("cancelEmpowermentFromMe", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCancelApi.cancelEmpowermentFromMe(
                empowermentId = empowermentId,
                request = EmpowermentCancelRequestModel(
                    reason = reason,
                )
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("cancelEmpowermentFromMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("cancelEmpowermentFromMe onFailure", message, TAG)
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

    override fun cancelEmpowermentToMe(
        reason: String,
        empowermentId: String
    ) = flow {
        logDebug("cancelEmpowermentToMe", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            empowermentCancelApi.cancelEmpowermentToMe(
                empowermentId = empowermentId,
                request = EmpowermentCancelRequestModel(
                    reason = reason,
                )
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("cancelEmpowermentToMe onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("cancelEmpowermentToMe onFailure", message, TAG)
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