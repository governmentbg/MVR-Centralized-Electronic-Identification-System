/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.notifications.channels

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.notifications.channels.NotificationChannelsSelectedResponse
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsSelectedModel

class NotificationChannelsSelectedResponseMapper :
    BaseMapper<NotificationChannelsSelectedResponse, NotificationChannelsSelectedModel>() {

    override fun map(from: NotificationChannelsSelectedResponse): NotificationChannelsSelectedModel {
        return with(from) {
            NotificationChannelsSelectedModel(
                pageIndex = pageIndex,
                totalItems = totalItems,
                data = data ?: emptyList()
            )
        }
    }

}