/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.database.notifications.channels

import android.os.Parcelable
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelModel
import kotlinx.coroutines.flow.Flow

interface NotificationChannelsDatabaseRepository : Parcelable {

    fun saveNotificationChannels(list: List<NotificationChannelModel>)

    fun saveSelectedNotificationChannels(list: List<String>)

    fun clearNotificationChannels()

    fun clearSelectedNotificationChannels()

    fun subscribeToNotificationChannels(): Flow<List<NotificationChannelModel>>

    fun subscribeToSelectedNotificationChannels(): Flow<List<String>>

    fun getSelectedNotificationChannels(): List<String>

}