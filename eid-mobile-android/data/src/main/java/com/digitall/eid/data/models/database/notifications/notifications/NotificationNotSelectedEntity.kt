/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.notifications

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "notification_not_selected")
data class NotificationNotSelectedEntity(
    @PrimaryKey val id: String,
)
