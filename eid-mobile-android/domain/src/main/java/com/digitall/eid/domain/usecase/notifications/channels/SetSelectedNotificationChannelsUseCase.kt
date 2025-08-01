/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.notifications.channels

import com.digitall.eid.domain.repository.network.notifications.channels.NotificationChannelsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class SetSelectedNotificationChannelsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "SetSelectedNotificationChannelsUseCaseTag"
        private const val PAGE_SIZE = 50
    }

    private val notificationChannelsNetworkRepository: NotificationChannelsNetworkRepository by inject()

    fun invoke(ids: List<String>) =
        notificationChannelsNetworkRepository.setSelectedNotificationChannels(
            ids
        )

}