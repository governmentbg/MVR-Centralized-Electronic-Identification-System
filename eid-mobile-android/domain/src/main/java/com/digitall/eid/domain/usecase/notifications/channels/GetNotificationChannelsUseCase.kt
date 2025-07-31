/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.notifications.channels

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsModel
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsSelectedModel
import com.digitall.eid.domain.models.notifications.channels.NotificationsChannelsModel
import com.digitall.eid.domain.repository.network.notifications.channels.NotificationChannelsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject
import kotlin.concurrent.Volatile

class GetNotificationChannelsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "UpdateNotificationChannelsUseCaseTag"
        private const val NOTIFICATION_CHANNELS_PAGE_SIZE = 100
        private const val SELECTED_NOTIFICATION_CHANNELS_PAGE_SIZE = 1000
    }

    private val notificationChannelsNetworkRepository: NotificationChannelsNetworkRepository by inject()

    @Volatile
    private var notificationsChannelsModel = NotificationsChannelsModel()

    fun invoke() = flow {
        combine(
            notificationChannelsNetworkRepository.getNotificationChannels(
                pageSize = NOTIFICATION_CHANNELS_PAGE_SIZE,
                pageIndex = 1,
                channelName = null
            ),
            notificationChannelsNetworkRepository.getSelectedNotificationChannels(
                pageSize = SELECTED_NOTIFICATION_CHANNELS_PAGE_SIZE,
                pageIndex = 1
            )
        ) { results -> results }.collect { results ->
            when {
                results.all { it.status == ResultEmittedData.Status.LOADING } -> emit(
                    ResultEmittedData.loading(model = null)
                )

                results.all { it.status == ResultEmittedData.Status.SUCCESS } -> {
                    results.onEach { model ->
                        when (model.model) {
                            is NotificationChannelsModel -> notificationsChannelsModel =
                                notificationsChannelsModel.copy(
                                    channels = model.model.data,
                                )

                            is NotificationChannelsSelectedModel -> notificationsChannelsModel =
                                notificationsChannelsModel.copy(
                                    enabledChannels = model.model.data
                                )
                        }
                    }

                    emit(
                        ResultEmittedData.success(
                            model = notificationsChannelsModel,
                            message = results.first().message,
                            responseCode = results.first().responseCode
                        )
                    )
                }

                results.any { it.status == ResultEmittedData.Status.ERROR } -> {
                    results.firstOrNull { it.status == ResultEmittedData.Status.ERROR }
                        ?.let { model ->
                            emit(
                                ResultEmittedData.error(
                                    model = null,
                                    error = model.error,
                                    title = model.title,
                                    message = model.message,
                                    errorType = model.errorType,
                                    responseCode = model.responseCode,
                                )
                            )
                        }
                }
            }
        }
    }.flowOn(Dispatchers.IO)
}