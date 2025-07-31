/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.dao.notifications.notifications

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Transaction
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEventEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationNotSelectedEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationWithEventsEntity
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow

@Dao
interface NotificationsDao {

    companion object {
        private const val TAG = "NotificationsDaoTag"
    }

    @Transaction
    @Query("UPDATE notifications SET isOpened = NOT isOpened WHERE id = :id")
    fun reverseOpenState(id: String)

    @Transaction
    fun saveNotificationsWithEvents(data: List<NotificationWithEventsEntity>) {
        logDebug("saveNotificationsWithEvents size: ${data.size}", TAG)
        saveNotifications(data.map { it.notification })
        saveNotificationEvents(data.flatMap { it.events })
    }

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun saveNotificationEvents(data: List<NotificationEventEntity>)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun saveNotifications(data: List<NotificationEntity>)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun saveNotificationNotSelectedEvents(data: List<NotificationNotSelectedEntity>)

    @Transaction
    @Query("SELECT * FROM notifications ORDER BY bulgarianName")
    fun subscribeToNotificationsWithEvents(): Flow<List<NotificationWithEventsEntity>>

    @Query("SELECT * FROM notification_not_selected")
    fun getNotSelectedNotifications(): List<NotificationNotSelectedEntity>

    @Transaction
    @Query("SELECT * FROM notifications WHERE id = :id")
    fun getNotificationById(id: String): NotificationWithEventsEntity

    @Transaction
    @Query("SELECT * FROM notification_not_selected")
    fun subscribeToNotSelectedEvents(): Flow<List<NotificationNotSelectedEntity>>

    @Query("DELETE FROM notifications")
    fun deleteNotifications()

    @Query("DELETE FROM notification_not_selected")
    fun deleteNotificationsNotSelected()

}