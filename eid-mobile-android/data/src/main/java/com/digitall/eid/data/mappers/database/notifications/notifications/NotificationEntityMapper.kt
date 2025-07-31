/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.database.notifications.notifications

import com.digitall.eid.data.mappers.base.BaseReverseMapper
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEventEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationWithEventsEntity
import com.digitall.eid.domain.models.notifications.notifications.NotificationEventModel
import com.digitall.eid.domain.models.notifications.notifications.NotificationModel

class NotificationEntityMapper :
    BaseReverseMapper<NotificationWithEventsEntity, NotificationModel>() {

    companion object {
        private const val TAG = "NotificationEntityMapperTag"
    }

    override fun map(from: NotificationWithEventsEntity): NotificationModel {
        return with(from.notification) {
            NotificationModel(
                id = id,
                name = name,
                isOpened = isOpened,
                selectionState = null,
                isDeleted = isDeleted,
                modifiedOn = modifiedOn,
                modifiedBy = modifiedBy,
                isApproved = isApproved,
                englishName = englishName,
                bulgarianName = bulgarianName,
                events = from.events.map(::mapNotificationEvents)
            )
        }
    }

    private fun mapNotificationEvents(from: NotificationEventEntity): NotificationEventModel {
        return with(from) {
            NotificationEventModel(
                id = id,
                code = code,
                isSelected = null,
                parentId = parentId,
                isDeleted = isDeleted,
                modifiedBy = modifiedBy,
                modifiedOn = modifiedOn,
                englishName = englishName,
                isMandatory = isMandatory,
                bulgarianName = bulgarianName,
            )
        }
    }

    override fun reverse(to: NotificationModel): NotificationWithEventsEntity {
        return with(to) {
            NotificationWithEventsEntity(
                notification = NotificationEntity(
                    id = id,
                    name = name,
                    isOpened = isOpened,
                    isDeleted = isDeleted,
                    modifiedOn = modifiedOn,
                    modifiedBy = modifiedBy,
                    isApproved = isApproved,
                    englishName = englishName,
                    bulgarianName = bulgarianName,
                ),
                events = events?.map(::reverseNotificationEvents) ?: emptyList(),
            )
        }
    }

    private fun reverseNotificationEvents(to: NotificationEventModel): NotificationEventEntity {
        return with(to) {
            NotificationEventEntity(
                id = id,
                code = code,
                parentId = parentId,
                isDeleted = isDeleted,
                modifiedBy = modifiedBy,
                modifiedOn = modifiedOn,
                englishName = englishName,
                isMandatory = isMandatory,
                bulgarianName = bulgarianName,
            )
        }
    }

}