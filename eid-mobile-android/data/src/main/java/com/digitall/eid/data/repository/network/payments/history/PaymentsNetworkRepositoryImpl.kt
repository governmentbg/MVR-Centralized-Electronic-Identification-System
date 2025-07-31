package com.digitall.eid.data.repository.network.payments.history

import com.digitall.eid.data.mappers.network.payments.history.response.PaymentsHistoryResponseMapper
import com.digitall.eid.data.network.payments.PaymentsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.payments.PaymentsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class PaymentsNetworkRepositoryImpl: PaymentsNetworkRepository, BaseRepository() {

    companion object {
        private const val TAG = "PaymentsNetworkRepositoryTag"
    }

    private val paymentsApi: PaymentsApi by inject()
    private val paymentsHistoryResponseMapper: PaymentsHistoryResponseMapper by inject()

    override fun getHistory()= flow {
        logDebug("getCertificates", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            paymentsApi.getHistory()
        }.onSuccess { model, message, responseCode ->
            logDebug("getCertificates onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = paymentsHistoryResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            LogUtil.logError("getCertificates onFailure", message, TAG)
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