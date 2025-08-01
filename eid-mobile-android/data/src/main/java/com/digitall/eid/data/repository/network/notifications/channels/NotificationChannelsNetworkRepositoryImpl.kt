/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.notifications.channels

import com.digitall.eid.data.mappers.network.notifications.channels.NotificationChannelsResponseMapper
import com.digitall.eid.data.mappers.network.notifications.channels.NotificationChannelsSelectedResponseMapper
import com.digitall.eid.data.network.notifications.NotificationsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.notifications.channels.NotificationChannelsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class NotificationChannelsNetworkRepositoryImpl :
    NotificationChannelsNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "NotificationsNetworkRepositoryTag"
    }

    private val notificationsApi: NotificationsApi by inject()
    private val notificationChannelsResponseMapper: NotificationChannelsResponseMapper by inject()
    private val notificationChannelsSelectedResponseMapper: NotificationChannelsSelectedResponseMapper by inject()

    override fun getNotificationChannels(
        pageSize: Int?,
        pageIndex: Int?,
        channelName: String?,
    ) = flow {
        logDebug("getNotificationChannels", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.getNotificationChannels(
                pageSize = pageSize,
                pageIndex = pageIndex,
                channelName = channelName,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getNotificationChannels onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = notificationChannelsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getNotificationChannels onFailure", message, TAG)
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

    override fun getSelectedNotificationChannels(
        pageSize: Int?,
        pageIndex: Int?,
    ) = flow {
        logDebug("getSelectedNotificationChannels", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.getSelectedNotificationChannels()
        }.onSuccess { model, message, responseCode ->
            logDebug("getSelectedNotificationChannels onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = notificationChannelsSelectedResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getSelectedNotificationChannels onFailure", message, TAG)
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

    override fun setSelectedNotificationChannels(
        selectedChannelsIDs: List<String>,
    ) = flow {
        logDebug("setSelectedNotificationChannels size: ${selectedChannelsIDs.size}", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.setSelectedNotificationChannels(
                request = selectedChannelsIDs
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("setSelectedNotificationChannels onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("setSelectedNotificationChannels onFailure", message, TAG)
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