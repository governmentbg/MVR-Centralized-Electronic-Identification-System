/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.channels

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "notification_channels_selected")
data class NotificationChannelSelectedEntity(
    @PrimaryKey val id: String,
)
