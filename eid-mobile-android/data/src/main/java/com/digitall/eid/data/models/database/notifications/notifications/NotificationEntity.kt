/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.notifications

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "notifications")
data class NotificationEntity(
    @PrimaryKey val id: String,
    val name: String?,
    val isOpened: Boolean,
    val modifiedOn: String?,
    val modifiedBy: String?,
    val isApproved: Boolean?,
    val isDeleted: Boolean?,
    val englishName: String?,
    val bulgarianName: String?,
)
