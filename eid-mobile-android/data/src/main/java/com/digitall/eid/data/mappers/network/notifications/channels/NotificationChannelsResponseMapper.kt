/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.notifications.channels

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.notifications.channels.NotificationChannelResponse
import com.digitall.eid.data.models.network.notifications.channels.NotificationChannelsGetResponse
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelModel
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelTranslationModel
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsModel

class NotificationChannelsResponseMapper :
    BaseMapper<NotificationChannelsGetResponse, NotificationChannelsModel>() {

    override fun map(from: NotificationChannelsGetResponse): NotificationChannelsModel {
        return with(from) {
            NotificationChannelsModel(
                pageIndex = pageIndex,
                totalItems = totalItems,
                data = data?.map(::mapItem)
            )
        }
    }

    private fun mapItem(
        from: NotificationChannelResponse,
    ): NotificationChannelModel {
        return with(from) {
            NotificationChannelModel(
                id = id,
                name = name,
                price = price,
                infoUrl = infoUrl,
                isEnabled = name != "SMTP",
                translations = translations?.map { translation ->
                    NotificationChannelTranslationModel(
                        language = translation.language,
                        name = translation.name,
                        description = translation.description
                    )
                },
            )
        }
    }

}