/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.notifications.notifications

import com.digitall.eid.domain.repository.database.notifications.notifications.NotificationsDatabaseRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import org.koin.core.component.inject

class ReverseNotificationOpenStateUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ReverseNotificationOpenStateUseCaseTag"
    }

    private val notificationsDatabaseRepository: NotificationsDatabaseRepository by inject()

    fun invoke(id: String) {
        logDebug("reverseNotificationOpenState id: $id", TAG)
        return notificationsDatabaseRepository.reverseNotificationOpenState(id)
    }
}