/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.notifications.notifications

import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.domain.models.notifications.notifications.NotificationModel
import com.digitall.eid.domain.repository.database.notifications.notifications.NotificationsDatabaseRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.combine
import org.koin.core.component.inject

class SubscribeToNotificationsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "SubscribeToNotificationsUseCaseTag"
    }

    private val notificationsDatabaseRepository: NotificationsDatabaseRepository by inject()

    fun invoke(): Flow<List<NotificationModel>> {
        logDebug("invoke", TAG)
        return combine(
            notificationsDatabaseRepository.subscribeToNotifications(),
            notificationsDatabaseRepository.subscribeTotNotSelectedNotifications()
        ) { notifications, notSelectedNotifications ->
            logDebug(
                "invoke combine notifications size: ${notifications.size} notSelectedNotifications size: ${notSelectedNotifications.size}",
                TAG
            )

            notifications.map { notification ->
                notification.events?.map { event ->
                    event.isSelected = !notSelectedNotifications.contains(event.id)
                }
                val events = notification.events ?: emptyList()
                val selectionState = when {
                    events.all { it.isSelected == true } -> {
                        if (events.all { it.isMandatory == true }) {
                            SelectionState.SELECTED_NOT_ACTIVE
                        } else {
                            SelectionState.SELECTED
                        }
                    }

                    events.any { it.isSelected == true } -> {
                        if (events.all { it.isMandatory == true }) {
                            SelectionState.INDETERMINATE_NOT_ACTIVE
                        } else {
                            SelectionState.INDETERMINATE
                        }
                    }

                    else -> {
                        if (events.all { it.isMandatory == true }) {
                            SelectionState.NOT_SELECTED_NOT_ACTIVE
                        } else {
                            SelectionState.NOT_SELECTED
                        }
                    }
                }
                notification.selectionState = selectionState
                notification
            }
        }
    }

}