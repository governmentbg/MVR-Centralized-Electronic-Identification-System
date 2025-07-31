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
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.FlowCollector
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class SetNotSelectedNotificationUseCase : BaseUseCase {

    companion object {
        private const val TAG = "SetSelectedNotificationChannelsUseCaseTag"
        private const val NOT_SELECTED_NOTIFICATIONS_PAGE_SIZE = 50
    }

    private val notificationsNetworkRepository: NotificationsNetworkRepository by inject()
    private val notificationsDatabaseRepository: NotificationsDatabaseRepository by inject()

    @Volatile
    private var errorState = false

    @Volatile
    private var clearNotSelectedNotifications = false

    private var notSelectedNotificationsPageIndex = 1
    private var notSelectedNotificationsTotalItems = 0

    fun invoke(id: String): Flow<ResultEmittedData<Unit>> = flow {
        logDebug("invoke id: $id", TAG)
        notSelectedNotificationsPageIndex = 1
        clearNotSelectedNotifications = true
        errorState = false
        val current = notificationsDatabaseRepository.getNotSelectedNotifications()
            .toMutableList()
        if (current.contains(id)) {
            current.remove(id)
        } else {
            current.add(id)
        }
        notificationsNetworkRepository.setNotSelectedNotifications(
            current,
        ).onEach { result ->
            result.onLoading {
                logDebug("setNotSelectedNotifications onLoading", TAG)
                emit(ResultEmittedData.loading(model = null))
            }.onSuccess { _, _, _ ->
                logDebug("setNotSelectedNotifications onSuccess", TAG)
                getNotSelectedNotificationEvents(this@flow)
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("setNotSelectedNotifications onFailure", message, TAG)
                errorState = true
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
        }.collect()
    }.flowOn(Dispatchers.IO)

    private suspend fun getNotSelectedNotificationEvents(
        flow: FlowCollector<ResultEmittedData<Unit>>
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
                    model.pageIndex < 1000 &&
                    model.totalItems != null &&
                    model.pageIndex * NOT_SELECTED_NOTIFICATIONS_PAGE_SIZE <= model.totalItems
                ) {
                    notSelectedNotificationsTotalItems = model.totalItems
                    notSelectedNotificationsPageIndex = model.pageIndex + 1
                    getNotSelectedNotificationEvents(flow)
                } else {
                    logDebug("getNotSelectedNotificationEvents Ready", TAG)
                    flow.emit(
                        ResultEmittedData.success(
                            model = Unit,
                            message = message,
                            responseCode = responseCode,
                        )
                    )
                }
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("getNotSelectedNotificationEvents onFailure", message, TAG)
                errorState = true
                flow.emit(
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