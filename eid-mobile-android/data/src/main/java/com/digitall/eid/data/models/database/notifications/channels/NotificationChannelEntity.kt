/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.channels

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "notification_channels")
data class NotificationChannelEntity(
    @PrimaryKey val id: String,
    val name: String?,
    val price: String?,
    val infoUrl: String?,
    val isEnabled: Boolean?,
    val englishName: String?,
    val bulgarianName: String?,
    val englishDescription: String?,
    val bulgarianDescription: String?,
)
