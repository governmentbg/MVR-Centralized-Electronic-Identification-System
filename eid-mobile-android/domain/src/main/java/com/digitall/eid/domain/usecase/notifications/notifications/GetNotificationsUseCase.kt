/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.notifications.notifications

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.repository.database.notifications.notifications.NotificationsDatabaseRepository
import com.digitall.eid.domain.repository.network.notifications.notifications.NotificationsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.channels.ProducerScope
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.channelFlow
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.koin.core.component.inject

class GetNotificationsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetNotificationsUseCaseTag"
        private const val NOTIFICATIONS_PAGE_SIZE = 50
        private const val NOTIFICATIONS_MAX_ELEMENTS = 100
        private const val NOT_SELECTED_NOTIFICATIONS_PAGE_SIZE = 50
        private const val NOT_SELECTED_NOTIFICATIONS_MAX_ELEMENTS = 100
    }

    private val notificationsNetworkRepository: NotificationsNetworkRepository by inject()
    private val notificationsDatabaseRepository: NotificationsDatabaseRepository by inject()

    @Volatile
    private var errorState = false

    @Volatile
    private var clearNotifications = false

    private var notificationsPageIndex = 1
    private var notificationsTotalItems = 0

    @Volatile
    private var getNotificationsReady = false

    @Volatile
    private var clearNotSelectedNotifications = false

    private var notSelectedNotificationsPageIndex = 1
    private var notSelectedNotificationsTotalItems = 0

    @Volatile
    private var getNotSelectedNotificationsReady = false

    fun invoke(): Flow<ResultEmittedData<Unit>> = channelFlow {
        logDebug("invoke", TAG)
        notificationsPageIndex = 1
        notSelectedNotificationsPageIndex = 1
        getNotificationsReady = false
        getNotSelectedNotificationsReady = false
        clearNotifications = true
        clearNotSelectedNotifications = true
        errorState = false
        send(ResultEmittedData.loading(null))
        coroutineScope {
            val notificationsJob = launch {
                getNotifications(this@channelFlow)
            }
            val notSelectedNotificationsJob = launch {
                getNotSelectedNotificationEvents(this@channelFlow)
            }
            notificationsJob.join()
            notSelectedNotificationsJob.join()
        }
    }

    private suspend fun getNotifications(
        flow: ProducerScope<ResultEmittedData<Unit>>
    ) {
        logDebug("getNotifications", TAG)
        if (errorState) {
            logError("getNotifications errorState", TAG)
            return
        }
        notificationsNetworkRepository.getNotifications(
            pageSize = NOTIFICATIONS_PAGE_SIZE,
            pageIndex = notificationsPageIndex,
            systemName = null,
            includeDeleted = false,
        ).onEach { result ->
            result.onLoading {
                logDebug("getNotifications onLoading", TAG)
            }.onSuccess { model, message, responseCode ->
                logDebug("getNotifications onSuccess", TAG)
                if (model.data != null) {
                    logDebug("getNotifications size: ${model.data.size}", TAG)
                    if (clearNotifications) {
                        notificationsDatabaseRepository.clearNotifications()
                        clearNotifications = false
                    }
                    notificationsDatabaseRepository.saveNotifications(model.data)
                }
                if (model.pageIndex != null &&
                    model.pageIndex < NOTIFICATIONS_MAX_ELEMENTS &&
                    model.totalItems != null &&
                    model.pageIndex * NOTIFICATIONS_PAGE_SIZE <= model.totalItems
                ) {
                    notificationsTotalItems = model.totalItems
                    notificationsPageIndex = model.pageIndex + 1
                    getNotifications(flow)
                } else {
                    logDebug("getNotifications Ready", TAG)
                    getNotificationsReady = true
                    if (getNotSelectedNotificationsReady) {
                        flow.send(
                            ResultEmittedData.success(
                                model = Unit,
                                message = message,
                                responseCode = responseCode,
                            )
                        )
                    }
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("getNotifications onFailure", message, TAG)
                errorState = true
                flow.send(
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
        }.collect()
    }

    private suspend fun getNotSelectedNotificationEvents(
        flow: ProducerScope<ResultEmittedData<Unit>>
    ) {
        logDebug("getNotSelectedNotificationEvents", TAG)
        if (errorState) {
            logError("getNotSelectedNotificationEvents errorState", TAG)
            return
        }
        notificationsNetworkRepository.getNotSelectedNotifications(
            pageSize = NOT_SELECTED_NOTIFICATIONS_PAGE_SIZE,
            pageIndex = notSelectedNotificationsPageIndex,
        ).onEach { result ->
            result.onLoading {
                logDebug("getNotSelectedNotificationEvents onLoading", TAG)
            }.onSuccess { model, message, responseCode ->
                logDebug("getNotSelectedNotificationEvents onSuccess", TAG)
                if (model.data != null) {
                    logDebug("getNotSelectedNotificationEvents size: ${model.data.size}", TAG)
                    if (clearNotSelectedNotifications) {
                        notificationsDatabaseRepository.clearNotSelectedNotifications()
                        clearNotSelectedNotifications = false
                    }
                    notificationsDatabaseRepository.saveNotificationNotSelectedEvents(model.data)
                }
                if (model.pageIndex != null &&
                    model.pageIndex < NOT_SELECTED_NOTIFICATIONS_MAX_ELEMENTS &&
                    model.totalItems != null &&
                    model.pageIndex * NOT_SELECTED_NOTIFICATIONS_PAGE_SIZE <= model.totalItems
                ) {
                    notSelectedNotificationsTotalItems = model.totalItems
                    notSelectedNotificationsPageIndex = model.pageIndex + 1
                    getNotSelectedNotificationEvents(flow)
                } else {
                    logDebug("getNotSelectedNotificationEvents Ready", TAG)
                    getNotSelectedNotificationsReady = true
                    if (getNotificationsReady) {
                        flow.send(
                            ResultEmittedData.success(
                                model = Unit,
                                message = message,
                                responseCode = responseCode,
                            )
                        )
                    }
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("getNotSelectedNotificationEvents onFailure", message, TAG)
                errorState = true
                flow.send(
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
        }.collect()
    }

}