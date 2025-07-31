/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.notifications.notifications

import com.digitall.eid.data.mappers.network.notifications.notifications.NotificationsNotSelectedResponseMapper
import com.digitall.eid.data.mappers.network.notifications.notifications.NotificationsResponseMapper
import com.digitall.eid.data.network.notifications.NotificationsApi
import com.digitall.eid.data.repository.network.base.BaseRepository
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.network.notifications.notifications.NotificationsNetworkRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class NotificationsNetworkRepositoryImpl :
    NotificationsNetworkRepository,
    BaseRepository() {

    companion object {
        private const val TAG = "NotificationsNetworkRepositoryTag"
    }

    private val notificationsApi: NotificationsApi by inject()
    private val notificationsResponseMapper: NotificationsResponseMapper by inject()
    private val notificationsNotSelectedResponseMapper: NotificationsNotSelectedResponseMapper by inject()

    override fun getNotifications(
        pageSize: Int?,
        pageIndex: Int?,
        systemName: String?,
        includeDeleted: Boolean?,
    ) = flow {
        logDebug("getNotifications", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.getNotifications(
                pageSize = pageSize,
                pageIndex = pageIndex,
                systemName = systemName,
                includeDeleted = null,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getNotifications onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = notificationsResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getNotifications onFailure", message, TAG)
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

    override fun getNotSelectedNotifications(
        pageSize: Int?,
        pageIndex: Int?
    ) = flow {
        logDebug("getNotSelectedNotifications", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.getNotSelectedNotifications(
                pageSize = pageSize,
                pageIndex = pageIndex,
            )
        }.onSuccess { model, message, responseCode ->
            logDebug("getNotSelectedNotifications onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = notificationsNotSelectedResponseMapper.map(model),
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("getNotSelectedNotifications onFailure", message, TAG)
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

    override fun setNotSelectedNotifications(
        notSelectedNotificationsIDs: List<String>,
    ) = flow {
        logDebug("setNotSelectedNotifications size: ${notSelectedNotificationsIDs.size}", TAG)
        emit(ResultEmittedData.loading(model = null))
        getResult {
            notificationsApi.setNotSelectedNotifications(
                request = notSelectedNotificationsIDs
            )
        }.onSuccess { _, message, responseCode ->
            logDebug("setNotSelectedNotifications onSuccess", TAG)
            emit(
                ResultEmittedData.success(
                    model = Unit,
                    message = message,
                    responseCode = responseCode,
                )
            )
        }.onFailure { error, title, message, responseCode, errorType ->
            logError("setNotSelectedNotifications onFailure", message, TAG)
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