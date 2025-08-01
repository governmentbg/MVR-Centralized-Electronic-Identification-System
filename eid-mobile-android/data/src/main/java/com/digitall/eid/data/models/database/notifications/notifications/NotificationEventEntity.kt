/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.notifications

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "notification_events")
data class NotificationEventEntity(
    @PrimaryKey val id: String,
    val code: String?,
    val parentId: String?,
    val modifiedOn: String?,
    val modifiedBy: String?,
    val isDeleted: Boolean?,
    val englishName: String?,
    val isMandatory: Boolean?,
    val bulgarianName: String?,
)
