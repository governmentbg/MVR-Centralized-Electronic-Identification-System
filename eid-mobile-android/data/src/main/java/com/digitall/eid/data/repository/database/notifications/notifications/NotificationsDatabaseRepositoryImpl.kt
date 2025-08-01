/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.database.notifications.notifications

import com.digitall.eid.data.database.dao.notifications.notifications.NotificationsDao
import com.digitall.eid.data.mappers.database.notifications.notifications.NotificationEntityMapper
import com.digitall.eid.data.mappers.database.notifications.notifications.NotificationsNotSelectedEntityMapper
import com.digitall.eid.domain.models.notifications.notifications.NotificationModel
import com.digitall.eid.domain.repository.database.notifications.notifications.NotificationsDatabaseRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class NotificationsDatabaseRepositoryImpl :
    NotificationsDatabaseRepository,
    KoinComponent {

    companion object {
        private const val TAG = "NotificationsDatabaseRepositoryTag"
    }

    private val notificationsDao: NotificationsDao by inject()
    private val notificationEntityMapper: NotificationEntityMapper by inject()
    private val notificationsNotSelectedEntityMapper: NotificationsNotSelectedEntityMapper by inject()

    override fun saveNotifications(list: List<NotificationModel>) {
        logDebug("saveNotifications size: ${list.size}", TAG)
        notificationsDao.saveNotificationsWithEvents(notificationEntityMapper.reverseList(list))
    }

    override fun saveNotificationNotSelectedEvents(id: List<String>) {
        logDebug("saveNotificationEventSelectedState size: ${id.size}", TAG)
        notificationsDao.saveNotificationNotSelectedEvents(
            notificationsNotSelectedEntityMapper.reverseList(id)
        )
    }

    override fun subscribeToNotifications(): Flow<List<NotificationModel>> {
        logDebug("subscribeToNotifications", TAG)
        return notificationsDao.subscribeToNotificationsWithEvents()
            .map(notificationEntityMapper::mapList)
    }

    override fun subscribeTotNotSelectedNotifications(): Flow<List<String>> {
        logDebug("subscribeTotNotSelectedNotifications", TAG)
        return notificationsDao.subscribeToNotSelectedEvents()
            .map(notificationsNotSelectedEntityMapper::mapList)
    }

    override fun getNotSelectedNotifications(): List<String> {
        logDebug("getNotSelectedNotifications", TAG)
        return notificationsDao.getNotSelectedNotifications()
            .map(notificationsNotSelectedEntityMapper::map)
    }

    override fun getNotificationById(id: String): NotificationModel {
        logDebug("getNotificationById id: $id", TAG)
        return notificationEntityMapper.map(notificationsDao.getNotificationById(id = id))
    }

    override fun clearNotifications() {
        logDebug("clearNotifications", TAG)
        notificationsDao.deleteNotifications()
    }

    override fun clearNotSelectedNotifications() {
        logDebug("clearNotSelectedNotifications", TAG)
        notificationsDao.deleteNotificationsNotSelected()
    }

    override fun reverseNotificationOpenState(id: String) {
        notificationsDao.reverseOpenState(id)
    }

}