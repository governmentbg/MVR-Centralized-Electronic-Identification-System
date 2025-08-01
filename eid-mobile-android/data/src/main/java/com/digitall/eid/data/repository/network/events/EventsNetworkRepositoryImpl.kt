package com.digitall.eid.data.repository.network.events

import com.digitall.eid.data.mappers.network.events.request.EventsRequestMapper
import com.digitall.eid.data.network.events.EventsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.events.request.EventRequestModel
import com.digitall.eid.domain.repository.network.events.EventsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class EventsNetworkRepositoryImpl :
    EventsNetworkRepository,
    BaseRepository() {

        companion object {
            private const val TAG = "EventsNetworkRepositoryTag"
        }

    private val eventsApi: EventsApi by inject()

    private val eventsRequestMapper: EventsRequestMapper by inject()

    override fun logEvent(data: EventRequestModel) = flow {
        logDebug("logEvent", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            eventsApi.logEvent(request = eventsRequestMapper.map(data))
        }.onSuccess { _, message, responseCode ->
            logDebug("logEvent onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError(
                "logEvent onFailure",
                message, TAG
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