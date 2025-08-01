/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.notifications.notifications

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.notifications.notifications.NotificationsNotSelectedResponse
import com.digitall.eid.domain.models.notifications.notifications.NotificationsNotSelectedModel

class NotificationsNotSelectedResponseMapper :
    BaseMapper<NotificationsNotSelectedResponse, NotificationsNotSelectedModel>() {

    override fun map(from: NotificationsNotSelectedResponse): NotificationsNotSelectedModel {
        return with(from) {
            NotificationsNotSelectedModel(
                pageIndex = pageIndex,
                totalItems = totalItems,
                data = data ?: emptyList()
            )
        }
    }

}