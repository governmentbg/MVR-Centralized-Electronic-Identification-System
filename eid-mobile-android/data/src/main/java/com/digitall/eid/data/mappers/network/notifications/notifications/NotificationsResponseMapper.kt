/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.notifications.notifications

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.notifications.notifications.NotificationsGetResponse
import com.digitall.eid.data.models.network.notifications.notifications.NotificationDataEventResponse
import com.digitall.eid.data.models.network.notifications.notifications.NotificationDataResponse
import com.digitall.eid.domain.models.common.Language
import com.digitall.eid.domain.models.notifications.notifications.NotificationsModel
import com.digitall.eid.domain.models.notifications.notifications.NotificationEventModel
import com.digitall.eid.domain.models.notifications.notifications.NotificationModel

class NotificationsResponseMapper :
    BaseMapper<NotificationsGetResponse, NotificationsModel>() {

    override fun map(from: NotificationsGetResponse): NotificationsModel {
        return with(from) {
            NotificationsModel(
                pageIndex = pageIndex,
                totalItems = totalItems,
                data = data?.map(::mapNotificationData),
            )
        }
    }

    private fun mapNotificationData(from: NotificationDataResponse): NotificationModel {
        return with(from) {
            NotificationModel(
                id = id!!,
                name = name,
                englishName = translations?.firstOrNull {
                    it.language == Language.ENGLISH.type
                }?.name ?: "Unknown",
                bulgarianName = translations?.firstOrNull {
                    it.language == Language.BULGARIAN.type
                }?.name ?: "Unknown",
                isOpened = false,
                isApproved = isApproved,
                isDeleted = isDeleted,
                modifiedBy = modifiedBy,
                modifiedOn = modifiedOn,
                selectionState = null,
                events = events?.map {
                    mapNotificationEvent(
                        from = it,
                        parentId = id,
                    )
                },
            )
        }
    }

    private fun mapNotificationEvent(
        parentId: String,
        from: NotificationDataEventResponse
    ): NotificationEventModel {
        return with(from) {
            NotificationEventModel(
                id = id!!,
                code = code,
                isSelected = null,
                parentId = parentId,
                isDeleted = isDeleted,
                modifiedBy = modifiedBy,
                modifiedOn = modifiedOn,
                isMandatory = isMandatory,
                englishName = translations?.firstOrNull {
                    it.language == Language.ENGLISH.type
                }?.shortDescription ?: "Unknown",
                bulgarianName = translations?.firstOrNull {
                    it.language == Language.BULGARIAN.type
                }?.shortDescription ?: "Unknown",
            )
        }
    }

}