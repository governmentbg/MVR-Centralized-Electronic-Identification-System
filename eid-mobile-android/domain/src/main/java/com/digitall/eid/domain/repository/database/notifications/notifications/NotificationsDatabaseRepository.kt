/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.database.notifications.notifications

import com.digitall.eid.domain.models.notifications.notifications.NotificationModel
import kotlinx.coroutines.flow.Flow

interface NotificationsDatabaseRepository {

    fun saveNotifications(list: List<NotificationModel>)

    fun saveNotificationNotSelectedEvents(id: List<String>)

    fun clearNotifications()

    fun clearNotSelectedNotifications()

    fun reverseNotificationOpenState(id: String)

    fun subscribeToNotifications(): Flow<List<NotificationModel>>

    fun subscribeTotNotSelectedNotifications(): Flow<List<String>>

    fun getNotSelectedNotifications(): List<String>

    fun getNotificationById(id: String): NotificationModel

}