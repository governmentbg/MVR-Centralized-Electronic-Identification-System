/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.database.notifications.notifications

import androidx.room.Embedded
import androidx.room.Relation

data class NotificationWithEventsEntity(
    @Embedded val notification: NotificationEntity,
    @Relation(
        parentColumn = "id",
        entityColumn = "parentId",
        entity = NotificationEventEntity::class,
    )
    val events: List<NotificationEventEntity>
)
