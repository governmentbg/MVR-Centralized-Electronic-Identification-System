/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.dao.notifications.channels

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.digitall.eid.data.models.database.notifications.channels.NotificationChannelEntity
import com.digitall.eid.data.models.database.notifications.channels.NotificationChannelSelectedEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface NotificationChannelsDao {

    @Query("SELECT * FROM notification_channels")
    fun subscribeToNotificationChannels(): Flow<List<NotificationChannelEntity>>

    @Query("SELECT * FROM notification_channels_selected")
    fun subscribeToSelectedNotificationChannels(): Flow<List<NotificationChannelSelectedEntity>>

    @Query("SELECT * FROM notification_channels_selected")
    fun getSelectedNotificationChannels(): List<NotificationChannelSelectedEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun saveNotificationChannels(list: List<NotificationChannelEntity>)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun saveNotificationChannelsSelected(list: List<NotificationChannelSelectedEntity>)

    @Query("DELETE FROM notification_channels")
    fun deleteNotificationChannels()

    @Query("DELETE FROM notification_channels_selected")
    fun deleteNotificationChannelsSelected()

}