package com.digitall.eid.data.repository.network.requests

import com.digitall.eid.data.mappers.network.requests.request.RequestOutcomeRequestMapper
import com.digitall.eid.data.mappers.network.requests.response.RequestsResponseMapper
import com.digitall.eid.data.network.requests.RequestsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.requests.request.RequestOutcomeRequestModel
import com.digitall.eid.domain.repository.network.requests.RequestsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class RequestsNetworkRepositoryImpl : RequestsNetworkRepository, BaseRepository() {

    companion object {
        private const val TAG = "RequestsNetworkRepositoryImplTag"
    }

    private val requestsApi: RequestsApi by inject()
    private val requestsResponseMapper: RequestsResponseMapper by inject()
    private val requestOutcomeRequestMapper: RequestOutcomeRequestMapper by inject()

    override fun getAll() = flow {
        logDebug("getAll", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            requestsApi.getAll()
        }.onSuccess { model, message, responseCode ->
            logDebug("getAll onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = requestsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getAll onFailure", TAG)
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

    override fun setRequestOutcome(
        requestId: String?,
        outcome: RequestOutcomeRequestModel
    ) = flow {
        logDebug("setRequestOutcome", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            requestsApi.setRequestOutcome(
                approvalRequestId = requestId,
                requestBody = requestOutcomeRequestMapper.map(outcome)
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("setRequestOutcome onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("setRequestOutcome onFailure", TAG)
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