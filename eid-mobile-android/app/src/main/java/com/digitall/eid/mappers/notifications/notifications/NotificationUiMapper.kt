/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.notifications.notifications

import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.domain.models.notifications.notifications.NotificationModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.notifications.notifications.NotificationAdapterMarker
import com.digitall.eid.models.notifications.notifications.NotificationChildUi
import com.digitall.eid.models.notifications.notifications.NotificationParentUi

class NotificationUiMapper :
    BaseMapperWithData<List<NotificationModel>, ApplicationLanguage, List<NotificationAdapterMarker>>() {

    companion object {
        private const val TAG = "NotificationUiMapperTag"
    }

    override fun map(
        from: List<NotificationModel>,
        data: ApplicationLanguage?
    ): List<NotificationAdapterMarker> {
        logDebug("map size: ${from.size}", TAG)
        return buildList {
            from.forEach { notification ->
                val notificationName = when(data) {
                    ApplicationLanguage.BG -> notification.bulgarianName
                    ApplicationLanguage.EN -> notification.englishName
                    null -> "Unknown"
                }
                add(
                    NotificationParentUi(
                        id = notification.id,
                        name = notificationName ?: "Unknown",
                        isOpened = notification.isOpened,
                        selectionState = notification.selectionState ?: SelectionState.NOT_SELECTED,
                    )
                )
                if (notification.isOpened) {
                    notification.events?.forEach { event ->
                        val eventName = when(data) {
                            ApplicationLanguage.BG -> event.bulgarianName
                            ApplicationLanguage.EN -> event.englishName
                            null -> "Unknown"
                        }
                        add(
                            NotificationChildUi(
                                id = event.id,
                                name = eventName ?: "Unknown",
                                parentId = notification.id,
                                isMandatory = event.isMandatory,
                                isSelected = event.isSelected ?: false,
                            )
                        )
                    }
                }
            }
        }
    }
}